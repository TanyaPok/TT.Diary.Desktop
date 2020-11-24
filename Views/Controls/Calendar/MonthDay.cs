using System;

namespace TT.Diary.Desktop.Views.Controls.Calendar
{
    public class MonthDay : IMonthCalendarData
    {
        public DateTime Date { get; set; }

        public double Productivity { get; set; }

        public string Text
        {
            get
            {
                return Date.Day.ToString();
            }
        }
    }
}
