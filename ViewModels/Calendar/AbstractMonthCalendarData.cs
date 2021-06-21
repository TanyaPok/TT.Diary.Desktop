using System;

namespace TT.Diary.Desktop.ViewModels.Calendar
{
    public abstract class AbstractMonthCalendarData
    {
        public DateTime Date { get; set; }

        public DayOfWeek DayOfWeek => Date.DayOfWeek;

        public virtual string Text => Date.Day.ToString();
    }
}