using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public class YearsViewModel : ViewModelBase
    {
        private int _selectedYear;

        public int SelectedYear
        {
            set => Set(ref _selectedYear, value);
            get => _selectedYear;
        }

        public IList<int> Years { get; } = new List<int>();

        public YearsViewModel()
        {
            var startYear = DateTime.Now.AddYears(-5).Year;
            var endYear = DateTime.Now.AddYears(5).Year;

            for (var year = startYear; year <= endYear; year++)
            {
                Years.Add(year);
            }

            SelectedYear = DateTime.Now.Year;
        }
    }
}