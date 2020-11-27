using System;

namespace TT.Diary.Desktop.Views.Controls.Calendar
{
    public class Cap : IMonthCalendarData
    {
        public string Text { get; set; }

        public DayOfWeek DayOfWeek => throw new NotImplementedException();
    }
}
