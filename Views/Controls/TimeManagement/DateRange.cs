using GalaSoft.MvvmLight;
using System;

namespace TT.Diary.Desktop.Views.Controls.TimeManagement
{
    public class DateRange : ObservableObject
    {
        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                Set(ref _startDate, value);
            }
        }

        private DateTime? _finishDate;
        public DateTime? FinishDate
        {
            get
            {
                return _finishDate;
            }
            set
            {
                Set(ref _finishDate, value);
            }
        }

        public int InvocationListMethodsCount
        {
            get
            {
                return PropertyChangedHandler == null ? 0 : PropertyChangedHandler.GetInvocationList().Length;
            }
        }
    }
}
