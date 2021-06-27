using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Calendar;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class YearlySchedule : AbstractContentControlViewModel
    {
        private readonly int _noTodo = -1;
        private List<MonthDay> _data;

        public override string Title => "Year";
        public YearsViewModel YearsViewModel { get; }

        #region months

        private readonly ObservableCollection<AbstractMonthCalendarData> _january =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> January
        {
            get
            {
                FormMonth(_january, 1);
                return _january;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _february =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> February
        {
            get
            {
                FormMonth(_february, 2);
                return _february;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _march =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> March
        {
            get
            {
                FormMonth(_march, 3);
                return _march;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _april =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> April
        {
            get
            {
                FormMonth(_april, 4);
                return _april;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _may =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> May
        {
            get
            {
                FormMonth(_may, 5);
                return _may;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _june =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> June
        {
            get
            {
                FormMonth(_june, 6);
                return _june;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _july =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> July
        {
            get
            {
                FormMonth(_july, 7);
                return _july;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _august =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> August
        {
            get
            {
                FormMonth(_august, 8);
                return _august;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _september =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> September
        {
            get
            {
                FormMonth(_september, 9);
                return _september;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _october =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> October
        {
            get
            {
                FormMonth(_october, 10);
                return _october;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _november =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> November
        {
            get
            {
                FormMonth(_november, 11);
                return _november;
            }
        }

        private readonly ObservableCollection<AbstractMonthCalendarData> _december =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> December
        {
            get
            {
                FormMonth(_december, 12);
                return _december;
            }
        }

        #endregion

        public ICommand YearChangedCommand { get; }

        public YearlySchedule(int userId) : base(userId)
        {
            YearsViewModel = new YearsViewModel();
            YearChangedCommand = new RelayCommand(async () => { await YearChanged(); }, canExecute: () => true);
        }

        protected override async Task LoadDataAsync()
        {
            var finishDate = DateTime.Now.Year == YearsViewModel.SelectedYear
                ? DateTime.Now.AddDays(-1)
                : new DateTime(DateTime.Now.Year, 12, 31);
            var requestUri = string.Format(
                ServiceOperationContract.GetYearlySchedule,
                UserId,
                new DateTime(YearsViewModel.SelectedYear, 1, 1).ToString(ServiceOperationContract.DateFormat),
                finishDate.ToString(ServiceOperationContract.DateFormat));
            _data = await Endpoint.GetAsync<List<MonthDay>>(requestUri, ErrorMessages.GetSchedule.GetDescription(),
                YearsViewModel.SelectedYear);
        }

        protected override bool InRangeDates(DateRange dateRange)
        {
            return YearsViewModel.SelectedYear >= dateRange.StartDate.Year &&
                   YearsViewModel.SelectedYear <= dateRange.FinishDate.Year;
        }

        protected override async Task DataSettingAsync()
        {
            await LoadDataAsync();
        }

        private async Task YearChanged()
        {
            await DataSettingAsync();
            RaisePropertyChanged(nameof(January));
            RaisePropertyChanged(nameof(February));
            RaisePropertyChanged(nameof(March));
            RaisePropertyChanged(nameof(April));
            RaisePropertyChanged(nameof(May));
            RaisePropertyChanged(nameof(June));
            RaisePropertyChanged(nameof(July));
            RaisePropertyChanged(nameof(August));
            RaisePropertyChanged(nameof(September));
            RaisePropertyChanged(nameof(October));
            RaisePropertyChanged(nameof(November));
            RaisePropertyChanged(nameof(December));
        }

        private void FormMonth(ICollection<AbstractMonthCalendarData> days, int month)
        {
            days.Clear();
            var daysCount = DateTime.DaysInMonth(YearsViewModel.SelectedYear, month);

            for (var day = 1; day <= daysCount; day++)
            {
                var date = new DateTime(YearsViewModel.SelectedYear, month, day);
                days.Add(new MonthDay {Date = date, Productivity = CalcProductivity(date)});
            }
        }

        private double CalcProductivity(DateTime date)
        {
            var item = _data.FirstOrDefault(d => d.Date == date);

            if (item == null)
            {
                return _noTodo;
            }

            return item.Productivity;
        }
    }
}