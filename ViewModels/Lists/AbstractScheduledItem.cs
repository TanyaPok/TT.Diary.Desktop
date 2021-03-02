using System.ComponentModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractScheduledItem<T> : AbstractListItem, IPublisher<DirtyData> where T : AbstractScheduleSettings
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

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            TrackersRemoveCommand = new DeleteCommand<AbstractScheduledItem<T>>(async (owner) => { await RemoveTrackers(owner); }, CanRemoveTrackers, "trackers", true);
        }

        internal override async Task Remove()
        {
            if (Schedule != null && Schedule.State == EntityState.New && Schedule.CanRemove())
            {
                await Schedule.Remove();
                Schedule = null;
            }

            if (State == EntityState.New)
            {
                Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
                return;
            }

            await base.Remove();

            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
        }

        protected override async Task Save()
        {
            await base.Save();

            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });

            if (Schedule == null)
            {
                return;
            }

            Schedule.ParentId = Id;

            if (Schedule.CanAcceptChanges())
            {
                await Schedule.AcceptChanges();
            }
        }

        internal virtual async Task Reschedule()
        {
            if (!Schedule.CanRemove())
            {
                return;
            }

            await Schedule.Remove();
        }

        internal virtual async Task Complete()
        {
            Schedule.CompletionDate = Schedule.CalculateCompletionDate();

            if (!Schedule.CanAcceptChanges())
            {
                return;
            }

            await Schedule.AcceptChanges();
            RaisePropertyChanged(nameof(IsCompleted));
        }

        internal override bool CanRemove()
        {
            return Schedule == null || Schedule.State == EntityState.New;
        }

        protected virtual bool CanRemoveTrackers(AbstractScheduledItem<T> owner)
        {
            return owner == this && IsTracked;
        }

        protected abstract Task RemoveTrackers(AbstractScheduledItem<T> owner);

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTracked) || e.PropertyName == nameof(IsCompleted))
            {
                return;
            }

            base.EntityPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Schedule) && Schedule == null)
            {
                return;
            }

            Notify(new DirtyData { Source = this, Operation = OperationType.Add });
        }
    }
}
