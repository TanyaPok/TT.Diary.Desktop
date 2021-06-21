using System;

namespace TT.Diary.Desktop.ViewModels.Calendar
{
    public class Cap : AbstractMonthCalendarData
    {
        public string Head { get; set; }

        public override string Text => Head;

        public Cap()
        {
            Date = DateTime.MinValue;
        }
    }
}