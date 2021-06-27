using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.TimeManagement;
using TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class WeeklySchedule : AbstractContentControlViewModel
    {
        public override string Title => "Weekly schedule";

        private DateRange _selectedDateRange;

        public DateRange SelectedDateRange
        {
            set => Set(ref _selectedDateRange, value);
            get => _selectedDateRange;
        }

        public ObservableCollection<DateRange> DateRanges { get; } = new ObservableCollection<DateRange>();

        public MonthsViewModel MonthsViewModel { get; }

        #region PlannerFrames

        private ScheduledToDoPlannerFrame _toDoPlanner;

        public ScheduledToDoPlannerFrame ToDoPlanner
        {
            get => _toDoPlanner;
            set => Set(ref _toDoPlanner, value);
        }

        private ScheduledAppointmentPlannerFrame _appointmentPlanner;

        public ScheduledAppointmentPlannerFrame AppointmentPlanner
        {
            get => _appointmentPlanner;
            set => Set(ref _appointmentPlanner, value);
        }

        private ScheduledHabitPlannerFrame _habitPlanner;

        public ScheduledHabitPlannerFrame HabitPlanner
        {
            get => _habitPlanner;
            set => Set(ref _habitPlanner, value);
        }

        private ScheduledWishPlannerFrame _wishPlanner;

        public ScheduledWishPlannerFrame WishPlanner
        {
            get => _wishPlanner;
            set => Set(ref _wishPlanner, value);
        }

        #endregion

        public ICommand DateRangeChangedCommand { get; }

        public WeeklySchedule(int userId) : base(userId)
        {
            MonthsViewModel = new MonthsViewModel();

            ConfigureDateRanges(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key);
            SelectedDateRange =
                DateRanges.First(d => d.StartDate <= DateTime.Now.Date && d.FinishDate >= DateTime.Now.Date);

            MonthsViewModel.PropertyChanged += MonthsViewModel_PropertyChanged;
            DateRangeChangedCommand =
                new RelayCommand(async () => { await DateRangeChanged(); }, () => true);

            HabitPlanner = ScheduledHabitPlannerFrame.Create(UserId);
            ToDoPlanner = ScheduledToDoPlannerFrame.Create(UserId);
            AppointmentPlanner = ScheduledAppointmentPlannerFrame.Create(UserId);
            WishPlanner = ScheduledWishPlannerFrame.Create(UserId);
        }

        protected override bool InRangeDates(DateRange dateRange)
        {
            return _selectedDateRange.StartDate.Date >= dateRange.StartDate.Date &&
                   _selectedDateRange.StartDate.Date <= dateRange.FinishDate.Date ||
                   _selectedDateRange.FinishDate.Date >= dateRange.StartDate.Date &&
                   _selectedDateRange.FinishDate.Date <= dateRange.FinishDate.Date ||
                   dateRange.StartDate.Date >= _selectedDateRange.StartDate.Date &&
                   dateRange.StartDate.Date <= _selectedDateRange.FinishDate.Date ||
                   dateRange.FinishDate.Date >= _selectedDateRange.StartDate.Date &&
                   dateRange.FinishDate.Date <= _selectedDateRange.FinishDate.Date;
        }

        protected override async Task LoadDataAsync()
        {
            var requestUri = string.Format(
                ServiceOperationContract.GetDailySchedule,
                UserId,
                SelectedDateRange.StartDate.Date.ToString(ServiceOperationContract.DateFormat),
                SelectedDateRange.FinishDate.Date.ToString(ServiceOperationContract.DateFormat));
            var planner = await Endpoint.GetAsync<Planner>(requestUri, ErrorMessages.GetSchedule.GetDescription(),
                $"{SelectedDateRange.StartDate.Date}-{SelectedDateRange.FinishDate.Date}");
            ToDoPlanner.Items.ReUpload(planner.ToDoList);
            AppointmentPlanner.Items.ReUpload(planner.Appointments);
            HabitPlanner.Items.ReUpload(planner.Habits);
            WishPlanner.Items.ReUpload(planner.WishList);
        }

        protected override Task DataSettingAsync()
        {
            HabitPlanner.SetDateRange(SelectedDateRange.StartDate, SelectedDateRange.FinishDate);
            ToDoPlanner.SetDateRange(SelectedDateRange.StartDate, SelectedDateRange.FinishDate);
            AppointmentPlanner.SetDateRange(SelectedDateRange.StartDate, SelectedDateRange.FinishDate);
            WishPlanner.SetDateRange(SelectedDateRange.StartDate, SelectedDateRange.FinishDate);
            return Task.CompletedTask;
        }

        private void ConfigureDateRanges(int year, int month)
        {
            var daysCount = DateTime.DaysInMonth(year, month);
            var startDay = new DateTime(year, month, 1);
            var finishDay = new DateTime(year, month, daysCount);

            DateRanges.Clear();

            do
            {
                var diff = 7 - (int) startDay.DayOfWeek;

                if (diff == 7)
                {
                    DateRanges.Add(new DateRange() {StartDate = startDay, FinishDate = startDay});
                }
                else
                {
                    var day = startDay.AddDays(diff);
                    var calcDay = (day > finishDay) ? finishDay : day;
                    DateRanges.Add(
                        new DateRange()
                        {
                            StartDate = startDay,
                            FinishDate = calcDay
                        });
                    startDay = calcDay;
                }

                startDay = startDay.AddDays(1);
            } while (startDay <= finishDay);
        }

        private void MonthsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ConfigureDateRanges(MonthsViewModel.SelectedYear, MonthsViewModel.SelectedMonth.Key);
            SelectedDateRange = DateRanges.First(d => d.StartDate.Day == 1);
        }

        private async Task DateRangeChanged()
        {
            if (SelectedDateRange == null) return;
            await LoadDataAsync();
            await DataSettingAsync();
        }
    }
}