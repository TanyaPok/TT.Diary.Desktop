using System;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractTrackedItemPlannerFrame<T, TP> : AbstractScheduledItemPlannerFrame<T, TP>
        where T : AbstractScheduledItem<ScheduleSettings>, new()
        where TP : UnscheduledItemSummary, new()
    {
        private readonly string _getTrackerOperation;

        protected AbstractTrackedItemPlannerFrame(int userId, string getUnscheduledItemsOperation,
            string getScheduleOperation, string getTrackerOperation)
            : base(userId, getUnscheduledItemsOperation, getScheduleOperation)
        {
            _getTrackerOperation =
                string.IsNullOrEmpty(getTrackerOperation) || string.IsNullOrWhiteSpace(getTrackerOperation)
                    ? throw new ArgumentNullException(nameof(getTrackerOperation))
                    : getTrackerOperation;
        }

        public override async Task AfterAcceptChanges(AbstractScheduledItem<ScheduleSettings> storable)
        {
            await base.AfterAcceptChanges(storable);
            PrepareTrackers((T) storable);
        }

        protected override void Prepare(T element)
        {
            base.Prepare(element);
            PrepareTrackers(element);
        }

        private void PrepareTrackers(T element)
        {
            foreach (var tracker in element.Schedule.Trackers)
            {
                tracker.ParentId = element.Id;
                tracker.OperationContract = _getTrackerOperation;
                tracker.OwnerType = (OwnerTypes) Enum.Parse(typeof(OwnerTypes), typeof(T).GetNameWithoutGenericArity());
            }
        }
    }
}