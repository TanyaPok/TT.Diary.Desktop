using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.Views.Controls.Calendar;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class YearlySchedule : AbstractContentControlViewModel
    {
        private readonly int _userId;
        private readonly int NO_TODO = -1;
        private List<MonthDay> _data;

        public string Title
        {
            get
            {
                return "Year";
            }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            set
            {
                Set(ref _selectedYear, value);
            }
            get
            {
                return _selectedYear;
            }
        }

        public IList<int> Years { get; } = new List<int>();

        #region months
        private ObservableCollection<IMonthCalendarData> _january = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> January
        {
            get
            {
                FormMonth(_january, 1);
                return _january;
            }
        }

        private ObservableCollection<IMonthCalendarData> _february = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> February
        {
            get
            {
                FormMonth(_february, 2);
                return _february;
            }
        }

        private ObservableCollection<IMonthCalendarData> _march = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> March
        {
            get
            {
                FormMonth(_march, 3);
                return _march;
            }
        }

        private ObservableCollection<IMonthCalendarData> _april = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> April
        {
            get
            {
                FormMonth(_april, 4);
                return _april;
            }
        }

        private ObservableCollection<IMonthCalendarData> _may = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> May
        {
            get
            {
                FormMonth(_may, 5);
                return _may;
            }
        }

        private ObservableCollection<IMonthCalendarData> _june = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> June
        {
            get
            {
                FormMonth(_june, 6);
                return _june;
            }
        }

        private ObservableCollection<IMonthCalendarData> _july = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> July
        {
            get
            {
                FormMonth(_july, 7);
                return _july;
            }
        }

        private ObservableCollection<IMonthCalendarData> _august = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> August
        {
            get
            {
                FormMonth(_august, 8);
                return _august;
            }
        }

        private ObservableCollection<IMonthCalendarData> _september = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> September
        {
            get
            {
                FormMonth(_september, 9);
                return _september;
            }
        }

        private ObservableCollection<IMonthCalendarData> _october = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> October
        {
            get
            {
                FormMonth(_october, 10);
                return _october;
            }
        }

        private ObservableCollection<IMonthCalendarData> _november = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> November
        {
            get
            {
                FormMonth(_november, 11);
                return _november;
            }
        }

        private ObservableCollection<IMonthCalendarData> _december = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> December
        {
            get
            {
                FormMonth(_december, 12);
                return _december;
            }
        }
        #endregion

        public YearlySchedule(int userId)
        {
            _userId = userId == INITIALIZATION_IDENTIFIER ? throw new ArgumentOutOfRangeException(nameof(userId)) : userId;

            var startYear = DateTime.Now.AddYears(-5).Year;
            var endYear = DateTime.Now.AddYears(5).Year;

            for (var year = startYear; year <= endYear; year++)
            {
                Years.Add(year);
            }

            SelectedYear = DateTime.Now.Year;
            PropertyChanged += YearlySchedule_PropertyChanged;
        }

        ~YearlySchedule()
        {
            PropertyChanged -= YearlySchedule_PropertyChanged;
        }

        protected override async Task LoadData()
        {
            var finishDate = DateTime.Now.Year == SelectedYear ? DateTime.Now.AddDays(-1) : new DateTime(DateTime.Now.Year, 12, 31);
            var requestUri = string.Format(
               ServiceOperationContract.GET_YEARLY_SCHEDULE,
               _userId,
               new DateTime(SelectedYear, 1, 1).ToString(ServiceOperationContract.DATE_FORMAT),
               finishDate.ToString(ServiceOperationContract.DATE_FORMAT));

            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    _data = await response.Content.ReadAsAsync<List<MonthDay>>();
                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetSchedule.GetDescription(), SelectedYear, response.StatusCode));
            }
        }

        protected override async Task DataSetting()
        {
            await LoadData();
        }

        private void YearlySchedule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedYear))
            {
                return;
            }

            Task.Run(() => DataSetting()).ConfigureAwait(true).GetAwaiter().GetResult();
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

        private void FormMonth(ObservableCollection<IMonthCalendarData> days, int month)
        {
            days.Clear();
            var daysCount = DateTime.DaysInMonth(SelectedYear, month);

            for (var day = 1; day <= daysCount; day++)
            {
                var date = new DateTime(SelectedYear, month, day);
                days.Add(new MonthDay { Date = date, Productivity = CalcProductivity(date) });
            }
        }

        private double CalcProductivity(DateTime date)
        {
            var item = _data.FirstOrDefault(d => d.Date == date);

            if (item == null)
            {
                return NO_TODO;
            }

            return item.Productivity;
        }
    }
}
