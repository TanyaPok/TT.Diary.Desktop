using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Commands.InitializingCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.TimeManagement;
using TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class DailySchedule : AbstractContentControlViewModel, ICreator
    {
        public override string Title => "Daily schedule for";

        private DateTime _previousSelectedDate = DateTime.Now;
        private DateTime _selectedDate = DateTime.Now;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _previousSelectedDate = _selectedDate;
                Set(ref _selectedDate, value);
            }
        }

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

        private NotePlannerFrame _notePlanner;

        public NotePlannerFrame NotePlanner
        {
            get => _notePlanner;
            set => Set(ref _notePlanner, value);
        }

        #endregion

        public ICommand SelectedDateChangedCommand { get; }

        public DailySchedule(int userId) : base(userId)
        {
            SelectedDateChangedCommand =
                new RelayCommand(async () => { await SelectedDateChanged(); }, () => IsConsistentState);

            NotePlanner = NotePlannerFrame.Create(UserId);
            HabitPlanner = ScheduledHabitPlannerFrame.Create(UserId);
            ToDoPlanner = ScheduledToDoPlannerFrame.Create(UserId);
            AppointmentPlanner = ScheduledAppointmentPlannerFrame.Create(UserId);
            WishPlanner = ScheduledWishPlannerFrame.Create(UserId);
        }

        public void RemoveNewNotSavedEntities()
        {
            NotePlanner.RemoveNewNotSavedEntities();
            HabitPlanner.RemoveNewNotSavedEntities();
            ToDoPlanner.RemoveNewNotSavedEntities();
            AppointmentPlanner.RemoveNewNotSavedEntities();
            WishPlanner.RemoveNewNotSavedEntities();
        }

        protected override bool InRangeDates(DateRange dateRange)
        {
            return dateRange.StartDate.Date <= SelectedDate.Date && dateRange.FinishDate.Date >= SelectedDate.Date;
        }

        protected override async Task LoadDataAsync()
        {
            var requestUri = string.Format(
                ServiceOperationContract.GetDailySchedule,
                UserId,
                SelectedDate.Date.ToString(ServiceOperationContract.DateFormat),
                SelectedDate.Date.ToString(ServiceOperationContract.DateFormat));
            var planner = await Endpoint.GetAsync<Planner>(requestUri, ErrorMessages.GetSchedule.GetDescription(),
                SelectedDate);
            ToDoPlanner.ReUploadItems(planner.ToDoList);
            AppointmentPlanner.ReUploadItems(planner.Appointments);
            HabitPlanner.ReUploadItems(planner.Habits);
            NotePlanner.ReUploadItems(planner.Notes);
            WishPlanner.ReUploadItems(planner.WishList);
        }

        protected override async Task DataSettingAsync()
        {
            NotePlanner.SetDateRange(SelectedDate, SelectedDate);
            HabitPlanner.SetDateRange(SelectedDate, SelectedDate);
            ToDoPlanner.SetDateRange(SelectedDate, SelectedDate);
            AppointmentPlanner.SetDateRange(SelectedDate, SelectedDate);
            WishPlanner.SetDateRange(SelectedDate, SelectedDate);
            await HabitPlanner.SetUnscheduledData();
            await ToDoPlanner.SetUnscheduledData();
            await AppointmentPlanner.SetUnscheduledData();
            await WishPlanner.SetUnscheduledData();
        }

        private bool CanSelectedDateChanged()
        {
            return IsConsistentState;
        }

        private async Task SelectedDateChanged()
        {
            if (_previousSelectedDate.Date == _selectedDate.Date)
            {
                return;
            }

            await LoadDataAsync();
            await DataSettingAsync();
        }
    }
}