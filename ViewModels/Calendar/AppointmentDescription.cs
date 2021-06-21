using System;

namespace TT.Diary.Desktop.ViewModels.Calendar
{
    public class AppointmentDescription
    {
        public DateTime Item1 { get; set; }

        public string Item2 { get; set; }

        public override string ToString()
        {
            return $"{Item1:HH:mm} {Item2}";
        }
    }
}