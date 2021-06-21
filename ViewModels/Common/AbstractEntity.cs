using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Commands.SaveCommands;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public enum EntityState
    {
        New,
        Changed,
        Saved
    }

    public abstract class AbstractEntity : ObservableObjectWithNotifyDataErrorInfo, IPublisher<DirtyData>, IStorable,
        IDisposable
    {
        internal int ParentId { get; set; }

        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                Set(ref _id, value);
                RaisePropertyChanged(nameof(State));
            }
        }

        public EntityState State
        {
            get
            {
                if (Id == default)
                {
                    return EntityState.New;
                }

                return IsChanged ? EntityState.Changed : EntityState.Saved;
            }
        }

        public int UserId { get; set; }
        public SaveCommand<AbstractEntity> SaveCommand { get; private set; }
        public bool IsChanged { get; private set; }

        #region implementation IStorable

        public virtual bool CanAcceptChanges()
        {
            return IsChanged && !HasErrors;
        }

        public virtual void AfterAcceptChanges()
        {
            IsChanged = false;
            SaveCommand?.RaiseCanExecuteChanged();
            Notify(new DirtyData {Source = this, Operation = OperationType.Remove});
        }

        public abstract Request GetCreateRequest();
        public abstract Request GetUpdateRequest();

        #endregion

        public virtual void Dispose()
        {
            PropertyChanged -= EntityPropertyChanged;
        }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        public async Task AcceptChanges()
        {
            await SaveCommand.ExecuteAsync(this);
        }

        internal void SubscribeToPropertyChanging()
        {
            PropertyChanged += EntityPropertyChanged;
        }

        internal virtual void GenerateCommands()
        {
            SaveCommand = new SaveCommand<AbstractEntity>(true);
        }

        protected virtual void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!TrackedPropertyNames.Contains(e.PropertyName))
            {
                return;
            }

            IsChanged = true;
            SaveCommand?.RaiseCanExecuteChanged();
            Notify(new DirtyData {Source = this, Operation = OperationType.Add});
        }
    }
}