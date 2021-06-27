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
    public class MonthlySchedule : AbstractContentControlViewModel
    {
        private List<DailyScheduledAppointments> _data;

        public override string Title => "Events and appointments";

        public MonthsViewModel MonthsViewModel { get; }

        private readonly ObservableCollection<AbstractMonthCalendarData> _days =
            new ObservableCollection<AbstractMonthCalendarData>();

        public ObservableCollection<AbstractMonthCalendarData> Days
        {
            get
            {
                _days.Clear();
                var finishDate = new DateTime(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key,
                    DateTime.DaysInMonth(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key));

                for (var day = 1; day <= finishDate.Day; day++)
                {
                    var date = new DateTime(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key, day);
                    _days.Add(GetAppointmentsAndEvents(date));
                }

                return _days;
            }
        }

        public ICommand MonthChangedCommand { get; }
        public ICommand YearChangedCommand { get; }

        public MonthlySchedule(int userId) : base(userId)
        {
            MonthsViewModel = new MonthsViewModel();
            MonthChangedCommand = new RelayCommand(async () => { await MonthChanged(); }, canExecute: () => true);
            YearChangedCommand = new RelayCommand(async () => { await YearChanged(); }, canExecute: () => true);
        }

        protected override bool InRangeDates(DateRange dateRange)
        {
            var startDate = new DateTime(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key, 1);
            var finishDate = new DateTime(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key,
                DateTime.DaysInMonth(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key));
            return startDate >= dateRange.StartDate.Date && startDate <= dateRange.FinishDate.Date ||
                   finishDate >= dateRange.StartDate.Date && finishDate <= dateRange.FinishDate.Date ||
                   dateRange.StartDate.Date >= startDate && dateRange.StartDate.Date <= finishDate ||
                   dateRange.FinishDate.Date >= startDate && dateRange.FinishDate.Date <= finishDate;
        }

        protected override async Task LoadDataAsync()
        {
            var requestUri = string.Format(
                ServiceOperationContract.GetMonthlySchedule,
                UserId,
                new DateTime(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key, 1).ToString(
                    ServiceOperationContract.DateFormat),
                new DateTime(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key,
                        DateTime.DaysInMonth(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key))
                    .ToString(ServiceOperationContract.DateFormat));
            _data = await Endpoint.GetAsync<List<DailyScheduledAppointments>>(requestUri,
                ErrorMessages.GetSchedule.GetDescription(),
                $"{MonthsViewModel.SelectedMonth.Value}, {MonthsViewModel.SelectedYear}");
        }

        protected override async Task DataSettingAsync()
        {
            await LoadDataAsync();
        }

        private DailyScheduledAppointments GetAppointmentsAndEvents(DateTime date)
        {
            var item = _data.FirstOrDefault(d => d.Date == date);

            if (item != null)
            {
                return item;
            }

            return new DailyScheduledAppointments()
            {
                Date = date,
                ScheduledAppointments = new List<AppointmentDescription>(),
                DoneAppointments = new List<AppointmentDescription>()
            };
        }

        private async Task YearChanged()
        {
            await DataSettingAsync();
            RaisePropertyChanged(nameof(Days));
        }

        private async Task MonthChanged()
        {
            await DataSettingAsync();
            RaisePropertyChanged(nameof(Days));
        }
    }
}