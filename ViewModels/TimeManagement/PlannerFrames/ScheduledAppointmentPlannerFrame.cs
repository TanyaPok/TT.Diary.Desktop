using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class ScheduledAppointmentPlannerFrame : AbstractTrackedItemPlannerFrame<Appointment<ScheduleSettings>, UnscheduledItemSummary>
    {
        public ScheduledAppointmentPlannerFrame(int userId, string getUnscheduledItemsOperation)
            : base(userId, getUnscheduledItemsOperation, ServiceOperationContract.APPOINTMENT_SCHEDULE, ServiceOperationContract.APPOINTMENT_TRACKER)
        {

        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var newAppointment = new Appointment<ScheduleSettings>
            {
                PreSave = PreSaveItem
            };
            Items.Add(newAppointment);

            var schedule = new ScheduleSettings();
            newAppointment.Schedule = schedule;

            schedule.OperationContract = ServiceOperationContract.APPOINTMENT_SCHEDULE;
            schedule.GenerateCommands();
            schedule.SubscribeToPropertyChanging();

            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.ScheduledCompletionDate = DateRange.StartDate;
            schedule.Repeat = Repeat.None;
            schedule.Every = 0;
            schedule.DaysAmount = 0;

            var tracker = new Tracker() { ScheduledDate = DateRange.StartDate };
            tracker.OperationContract = ServiceOperationContract.APPOINTMENT_TRACKER;
            schedule.Trackers.Add(tracker);
        }
    }
}
