using GalaSoft.MvvmLight;
using System;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public class DateRange : ObservableObject
    {
        private DateTime _startDate;

        public DateTime StartDate
        {
            get => _startDate;
            set => Set(ref _startDate, value);
        }

        private DateTime _finishDate;

        public DateTime FinishDate
        {
            get => _finishDate;
            set => Set(ref _finishDate, value);
        }

        public int InvocationListMethodsCount => PropertyChangedHandler?.GetInvocationList().Length ?? 0;
    }
}