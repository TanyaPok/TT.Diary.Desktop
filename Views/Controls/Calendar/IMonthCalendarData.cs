using System;

namespace TT.Diary.Desktop.Views.Controls.Calendar
{
    public interface IMonthCalendarData
    {
        DayOfWeek DayOfWeek { get; }

        string Text { get; }
    }
}
