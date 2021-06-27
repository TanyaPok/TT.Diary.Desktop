using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class
        ScheduledHabitPlannerFrame : AbstractTrackedItemPlannerFrame<Habit<ScheduleSettings>, UnscheduledHabitSummary>
    {
        public static ScheduledHabitPlannerFrame Create(int userId)
        {
            var frame = new ScheduledHabitPlannerFrame(userId);
            frame.GenerateCommands();
            return frame;
        }

        protected override void FillUnscheduledItemSummaries(IEnumerable<Habit<ScheduleSettings>> data)
        {
            foreach (var item in data)
            {
                UnscheduledItemSummaries.Add(
                    new UnscheduledHabitSummary()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Amount = item.Amount,
                        ParentId = item.ParentId
                    });
            }
        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var habit = new Habit<ScheduleSettings>();

            var schedule = new ScheduleSettings();
            habit.Schedule = schedule;

            var tracker = new Tracker
            {
                ScheduledDate = DateRange.StartDate, OperationContract = ServiceOperationContract.HabitTracker
            };
            schedule.Trackers.Add(tracker);

            Items.Add(habit);

            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.Repeat = Repeat.Daily;
            schedule.Every = 1;
        }

        protected override void TemplateChange(SelectionChangedEventArgs e)
        {
            base.TemplateChange(e);

            if (NewItem == null)
            {
                return;
            }

            NewItem.Amount = Template?.Amount;
        }

        private ScheduledHabitPlannerFrame(int userId) : base(userId,
            ServiceOperationContract.GetUnscheduledHabits, ServiceOperationContract.HabitSchedule,
            ServiceOperationContract.HabitTracker)
        {
        }
    }
}