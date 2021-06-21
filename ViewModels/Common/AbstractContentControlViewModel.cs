using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class AbstractContentControlViewModel : ViewModelBase
    {
        private bool _isDataLoaded;
        private IList<AbstractEntity> DirtyEntities { get; } = new List<AbstractEntity>();
        protected int UserId { get; }
        internal bool IsAutosaveCapable { get; set; }
        public bool IsConsistentState => DirtyEntities.Count == 0;
        public virtual string Title { get; } = string.Empty;

        /// <summary>
        /// Main toolbar commands
        /// </summary>
        public ObservableCollection<IAttributedCommand> Commands { get; } =
            new ObservableCollection<IAttributedCommand>();

        protected AbstractContentControlViewModel(int userId)
        {
            UserId = userId == default
                ? throw new ArgumentOutOfRangeException(nameof(userId))
                : userId;
        }

        internal void ReceiveMessage(DirtyData message)
        {
            switch (message.Operation)
            {
                case OperationType.Add:
                    if (!DirtyEntities.Contains(message.Source))
                    {
                        DirtyEntities.Add(message.Source);
                        RaisePropertyChanged(nameof(IsConsistentState));
                    }

                    break;
                case OperationType.Remove:
                    if (DirtyEntities.Contains(message.Source))
                    {
                        DirtyEntities.Remove(message.Source);
                        RaisePropertyChanged(nameof(IsConsistentState));
                    }

                    break;
                default:
                    throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                        nameof(message.Operation));
            }
        }

        internal void RequestRefreshData(DateRange dateRange)
        {
            if (InRangeDates(dateRange))
            {
                _isDataLoaded = false;
            }
        }

        internal async Task InitializeAsync()
        {
            if (_isDataLoaded)
            {
                return;
            }

            await LoadDataAsync();
            _isDataLoaded = true;
            await DataSettingAsync();
        }

        internal async Task SaveDirtyEntitiesAsync()
        {
            foreach (var entity in DirtyEntities.ToArray())
            {
                if (entity.CanAcceptChanges())
                {
                    await entity.AcceptChanges();
                }
            }
        }

        protected abstract bool InRangeDates(DateRange dateRange);

        protected abstract Task LoadDataAsync();

        protected abstract Task DataSettingAsync();
    }
}