using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractScheduledItem<T> : AbstractListItem where T : IScheduleSettings
    {
        private T _schedule;

        [TrackChange]
        public T Schedule
        {
            get => _schedule;
            set => Set(ref _schedule, value);
        }

        private bool _isTracked;

        public bool IsTracked
        {
            get => _isTracked;
            set => Set(ref _isTracked, value);
        }

        public override bool CanBeRemoved()
        {
            return (Schedule == null || Schedule.Id == default) && !IsTracked;
        }
    }
}