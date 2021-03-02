using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class AbstractContentControlViewModel : ViewModelBase
    {
        private bool _isDataLoaded;

        protected readonly int INITIALIZATION_IDENTIFIER = 0;
        
        internal IList<AbstractEntity> DirtyEntities { get; } = new List<AbstractEntity>();

        public bool IsConsistentState
        {
            get
            {
                return DirtyEntities.Count == 0;
            }
        }

        /// <summary>
        /// Main toolbar commands
        /// </summary>
        public ObservableCollection<IAttributedCommand> Commands { get; } = new ObservableCollection<IAttributedCommand>();

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
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(message.Operation));
            }
        }

        internal void RequestRefreshData(DateTime rangeStartDate, DateTime rangeFinishDate)
        {
            if (InRangeDates(rangeStartDate, rangeFinishDate))
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

            DirtyEntities.Clear();
            await LoadData();
            _isDataLoaded = true;
        }

        internal async Task PostInitialize()
        {
            await DataSetting();
        }

        protected void RefreshData()
        {
            DirtyEntities.Clear();
            Task.Run(() => LoadData()).ConfigureAwait(true).GetAwaiter().GetResult();
            Task.Run(() => DataSetting()).ConfigureAwait(true).GetAwaiter().GetResult();
        }

        protected virtual bool InRangeDates(DateTime rangeStartDate, DateTime rangeFinishDate)
        {
            return true;
        }

        protected abstract Task LoadData();

        protected abstract Task DataSetting();
    }
}