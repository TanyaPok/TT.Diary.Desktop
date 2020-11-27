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
    public class MonthlySchedule : ContentControlViewModel
    {
        private int _userId;
        private List<ScheduledAppointments> _data;

        public string Title
        {
            get
            {
                return "Events and appointments";
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

        private KeyValuePair<int, string> _selectedMonth;
        public KeyValuePair<int, string> SelectedMonth
        {
            set
            {
                Set(ref _selectedMonth, value);
            }
            get
            {
                return _selectedMonth;
            }
        }

        public IList<KeyValuePair<int, string>> Months { get; } = new List<KeyValuePair<int, string>>();

        private ObservableCollection<IMonthCalendarData> _days = new ObservableCollection<IMonthCalendarData>();
        public ObservableCollection<IMonthCalendarData> Days
        {
            get
            {
                _days.Clear();
                var daysCount = DateTime.DaysInMonth(SelectedYear, SelectedMonth.Key);

                for (var day = 1; day <= daysCount; day++)
                {
                    var date = new DateTime(SelectedYear, SelectedMonth.Key, day);
                    _days.Add(GetAppointmentsAndEvents(date));
                }

                return _days;
            }
        }

        public MonthlySchedule()
        {
            var startYear = DateTime.Now.AddYears(-5).Year;
            var endYear = DateTime.Now.AddYears(5).Year;

            for (var year = startYear; year <= endYear; year++)
            {
                Years.Add(year);
            }

            SelectedYear = DateTime.Now.Year;

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

            PropertyChanged += MonthlySchedule_PropertyChanged;
        }

        ~MonthlySchedule()
        {
            PropertyChanged -= MonthlySchedule_PropertyChanged;
        }

        protected override async Task LoadData(int userId)
        {
            _userId = userId;
            var requestUri = string.Format(
               OperationContract.SCHEDULE_REQUEST_FORMAT,
               OperationContract.GET_MONTHLY_SCHEDULE,
               userId,
               new DateTime(SelectedYear, SelectedMonth.Key, 1).ToString(DATE_FORMAT),
               new DateTime(SelectedYear, SelectedMonth.Key, DateTime.DaysInMonth(SelectedYear, SelectedMonth.Key)).ToString(DATE_FORMAT));

            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    _data = await response.Content.ReadAsAsync<List<ScheduledAppointments>>();
                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetSchedule.GetDescription(), string.Format("{0}, {1}", SelectedMonth.Value, SelectedYear), response.StatusCode));
            }
        }

        private async void MonthlySchedule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedYear) && e.PropertyName != nameof(SelectedMonth))
            {
                return;
            }

            await LoadData(_userId);

            RaisePropertyChanged(nameof(Days));
        }

        private ScheduledAppointments GetAppointmentsAndEvents(DateTime date)
        {
            var item = _data.FirstOrDefault(d => d.Date == date);

            if (item != null)
            {
                return item;
            }

            return new ScheduledAppointments()
            {
                Date = date,
                ScheduledAppointmentsDescriptions = new List<string>(),
                DoneAppointmentsDescriptions = new List<string>()
            };
        }
    }
}
