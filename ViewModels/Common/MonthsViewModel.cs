using System;
using System.Collections.Generic;
using System.Linq;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public class MonthsViewModel : YearsViewModel
    {
        private KeyValuePair<int, string> _selectedMonth;

        public KeyValuePair<int, string> SelectedMonth
        {
            set => Set(ref _selectedMonth, value);
            get => _selectedMonth;
        }

        public IList<KeyValuePair<int, string>> Months { get; } = new List<KeyValuePair<int, string>>();

        public MonthsViewModel()
        {
            Months.Add(new KeyValuePair<int, string>(1, "January"));
            Months.Add(new KeyValuePair<int, string>(2, "February"));
            Months.Add(new KeyValuePair<int, string>(3, "March"));
            Months.Add(new KeyValuePair<int, string>(4, "April"));
            Months.Add(new KeyValuePair<int, string>(5, "May"));
            Months.Add(new KeyValuePair<int, string>(6, "June"));
            Months.Add(new KeyValuePair<int, string>(7, "July"));
            Months.Add(new KeyValuePair<int, string>(8, "August"));
            Months.Add(new KeyValuePair<int, string>(9, "September"));
            Months.Add(new KeyValuePair<int, string>(10, "October"));
            Months.Add(new KeyValuePair<int, string>(11, "November"));
            Months.Add(new KeyValuePair<int, string>(12, "December"));

            SelectedMonth = Months.FirstOrDefault(m => m.Key == DateTime.Now.Month);
        }
    }
}