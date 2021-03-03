using System;
using System.Collections.Specialized;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractTrackedItemPlannerFrame<T, P> : AbstractScheduledItemPlannerFrame<T, P>
        where T : AbstractScheduledItem<ScheduleSettings>, new()
        where P : UnscheduledItemSummary, new()
    {
        private readonly string _getTrackerOperation;

        public AbstractTrackedItemPlannerFrame(int userId, string getUnscheduledItemsOperation, string getScheduleOperation, string getTrackerOperation)
            : base(userId, getUnscheduledItemsOperation, getScheduleOperation)
        {
            _getTrackerOperation = string.IsNullOrEmpty(getTrackerOperation) || string.IsNullOrWhiteSpace(getTrackerOperation) ?
                throw new ArgumentNullException(nameof(getTrackerOperation)) :
                getTrackerOperation;
        }

        protected override void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.Items_CollectionChanged(sender, e);

            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                var element = (T)item;

                if (element.Schedule == null)
                {
                    continue;
                }

                foreach (var tracker in element.Schedule.Trackers)
                {
                    tracker.ParentId = element.Id;
                    tracker.OperationContract = _getTrackerOperation;
                }
            }
        }
    }
}
