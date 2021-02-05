using System;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractScheduledItem<T> : AbstractListItem where T : AbstractScheduleSettings
    {
        private T _schedule;
        public T Schedule
        {
            get
            {
                return _schedule;
            }
            set
            {
                Set(ref _schedule, value);
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Schedule != null && Schedule.CompletionDate.HasValue;
            }
        }

        private bool _isTracked;
        public bool IsTracked
        {
            get
            {
                return _isTracked;
            }
            set
            {
                Set(ref _isTracked, value);
            }
        }

        public IAttributedCommand TrackersRemoveCommand { get; protected set; }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            TrackersRemoveCommand = new DeleteCommand<AbstractScheduledItem<T>>(async (owner) => { await RemoveTrackers(owner); }, CanRemoveTrackers, "trackers", true);
        }

        internal abstract Task Reschedule();

        protected virtual bool CanRemoveTrackers(AbstractScheduledItem<T> owner)
        {
            return owner == this && IsTracked;
        }

        protected abstract Task RemoveTrackers(AbstractScheduledItem<T> owner);
    }
}
