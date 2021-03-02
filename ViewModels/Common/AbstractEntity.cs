using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public enum EntityState
    {
        New,
        Changed,
        Saved
    }

    public abstract class AbstractEntity : ObservableObjectWithNotifyDataErrorInfo, IEntityCommands, IDisposable
    {
        protected readonly int INITIALIZATION_IDENTIFIER = 0;

        protected abstract string RemoveOperationContract { get; }

        internal int ParentId { get; set; }

        public EntityState State
        {
            get
            {
                if (Id == INITIALIZATION_IDENTIFIER)
                {
                    return EntityState.New;
                }

                if (IsChanged)
                {
                    return EntityState.Changed;
                }

                return EntityState.Saved;
            }
        }

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                Set(ref _id, value);
                RaisePropertyChanged(nameof(State));
            }
        }

        public int UserId { get; set; }

        #region implementation IEntityCommands
        public bool IsChanged { get; private set; }

        public IAttributedCommand SaveCommand { get; private set; }

        //TODO: bad idea, think
        internal delegate void BeforeSaving();

        internal BeforeSaving PreSave { get; set; }

        public virtual bool CanAcceptChanges()
        {
            return IsChanged && !HasErrors;
        }

        public async Task AcceptChanges()
        {
            if (PreSave != null)
            {
                PreSave();
            }

            await Save();
            IsChanged = false;
            SaveCommand?.RaiseCanExecuteChanged();
        }
        #endregion

        public virtual void Dispose()
        {
            PropertyChanged -= EntityPropertyChanged;
        }

        internal void SubscribeToPropertyChanging()
        {
            PropertyChanged += EntityPropertyChanged;
        }

        internal virtual void GenerateCommands()
        {
            SaveCommand = new SaveCommand(async () => { await AcceptChanges(); }, CanAcceptChanges, true);
        }

        internal virtual bool CanRemove()
        {
            return true;
        }

        internal virtual async Task Remove()
        {
            var requestUri = string.Format(ServiceOperationContract.REQUEST_FORMAT, RemoveOperationContract, Id);
            await Endpoint.RemoveEntity(new Request { OperationContract = requestUri });
        }

        protected abstract Request GetCreateRequest();

        protected abstract Request GetUpdateRequest();

        protected virtual async Task Save()
        {
            if (State == EntityState.New)
            {
                Id = await Endpoint.CreateEntity(GetCreateRequest());
            }
            else
            {
                await Endpoint.UpdateEntity(GetUpdateRequest());
            }
        }

        protected virtual void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
            SaveCommand?.RaiseCanExecuteChanged();
        }
    }
}
