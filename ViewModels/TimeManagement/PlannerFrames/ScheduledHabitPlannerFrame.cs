using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class ScheduledHabitPlannerFrame : AbstractScheduledItemPlannerFrame<Habit<ScheduleSettings>, UnscheduledHabitSummary>
    {
        public ScheduledHabitPlannerFrame(int userId, string getUnscheduledItemsOperation)
            : base(userId, getUnscheduledItemsOperation, ServiceOperationContract.HABIT_SCHEDULE, ServiceOperationContract.HABIT_TRACKER)
        {
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

            var newHabit = new Habit<ScheduleSettings>
            {
                PreSave = PreSaveItem
            };
            Items.Add(newHabit);

            var schedule = new ScheduleSettings();
            newHabit.Schedule = schedule;

            schedule.OperationContract = ServiceOperationContract.HABIT_SCHEDULE;
            schedule.GenerateCommands();
            schedule.SubscribeToPropertyChanging();

            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.Repeat = Repeat.Daily;
            schedule.Every = 1;

            var tracker = new Tracker() { ScheduledDate = DateRange.StartDate };
            tracker.OperationContract = ServiceOperationContract.HABIT_TRACKER;
            schedule.Trackers.Add(tracker);
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
    }
}
