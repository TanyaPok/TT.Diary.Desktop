using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class
        ScheduledToDoPlannerFrame : AbstractTrackedItemPlannerFrame<ToDo<ScheduleSettings>, UnscheduledItemSummary>
    {
        public ScheduledToDoPlannerFrame(int userId) : base(userId,
            ServiceOperationContract.GetUnscheduledTodoList, ServiceOperationContract.TodoSchedule,
            ServiceOperationContract.TodoTracker)
        {
        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var toDo = new ToDo<ScheduleSettings>();

            var schedule = new ScheduleSettings();
            toDo.Schedule = schedule;

            var tracker = new Tracker
            {
                ScheduledDate = DateRange.StartDate, OperationContract = ServiceOperationContract.TodoTracker
            };
            schedule.Trackers.Add(tracker);

            Items.Add(toDo);

            //here for setting schedule.IsChanged in true
            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.ScheduledCompletionDate = DateRange.StartDate;
            schedule.Repeat = Repeat.None;
            schedule.Every = 0;
            schedule.DaysAmount = 0;
        }
    }
}