using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class ScheduledToDoPlannerFrame : AbstractTrackedItemPlannerFrame<ToDo<ScheduleSettings>, UnscheduledItemSummary>
    {
        public ScheduledToDoPlannerFrame(int userId, string getUnscheduledItemsOperation)
            : base(userId, getUnscheduledItemsOperation, ServiceOperationContract.TODO_SCHEDULE, ServiceOperationContract.TODO_TRACKER)
        {

        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var newToDo = new ToDo<ScheduleSettings>
            {
                PreSave = PreSaveItem
            };
            Items.Add(newToDo);

            var schedule = new ScheduleSettings();
            newToDo.Schedule = schedule;

            schedule.OperationContract = ServiceOperationContract.TODO_SCHEDULE;
            schedule.GenerateCommands();
            schedule.SubscribeToPropertyChanging();

            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.ScheduledCompletionDate = DateRange.StartDate;
            schedule.Repeat = Repeat.None;
            schedule.Every = 0;
            schedule.DaysAmount = 0;

            var tracker = new Tracker() { ScheduledDate = DateRange.StartDate };
            tracker.OperationContract = ServiceOperationContract.TODO_TRACKER;
            schedule.Trackers.Add(tracker);
        }
    }
}
