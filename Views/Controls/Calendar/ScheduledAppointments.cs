using System;
using System.Collections.Generic;

namespace TT.Diary.Desktop.Views.Controls.Calendar
{
    public class ScheduledAppointments : IMonthCalendarData
    {
        public DateTime Date { get; set; }

        public List<string> ScheduledAppointmentsDescriptions { get; set; }

        public List<string> DoneAppointmentsDescriptions { get; set; }

        public string Text
        {
            get
            {
                return Date.Day.ToString();
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return Date.DayOfWeek;
            }
        }
    }
}
