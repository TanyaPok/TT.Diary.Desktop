using System.Collections.Generic;

namespace TT.Diary.Desktop.ViewModels.Calendar
{
    public class DailyScheduledAppointments : AbstractMonthCalendarData
    {
        public List<AppointmentDescription> ScheduledAppointments { get; set; }

        public List<AppointmentDescription> DoneAppointments { get; set; }
    }
}