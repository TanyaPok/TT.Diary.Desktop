using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class
        ScheduledAppointmentPlannerFrame : AbstractTrackedItemPlannerFrame<Appointment<ScheduleSettings>,
            UnscheduledItemSummary>
    {
        public ScheduledAppointmentPlannerFrame(int userId)
            : base(userId, ServiceOperationContract.GetUnscheduledAppointments,
                ServiceOperationContract.AppointmentSchedule, ServiceOperationContract.AppointmentTracker)
        {
        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var appointment = new Appointment<ScheduleSettings>();

            var schedule = new ScheduleSettings();
            appointment.Schedule = schedule;

            var tracker = new Tracker
            {
                ScheduledDate = DateRange.StartDate,
                OperationContract = ServiceOperationContract.AppointmentTracker
            };
            schedule.Trackers.Add(tracker);

            Items.Add(appointment);

            //here for setting schedule.IsChanged in true
            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.ScheduledCompletionDate = DateRange.StartDate;
            schedule.Repeat = Repeat.None;
            schedule.Every = 0;
            schedule.DaysAmount = 0;
        }
    }
}