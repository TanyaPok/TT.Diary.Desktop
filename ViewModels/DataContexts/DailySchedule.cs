using System;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.TimeManagement;
using TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class DailySchedule : AbstractContentControlViewModel
    {
        private readonly int _userId;

        public string Title
        {
            get
            {
                return "Daily schedule for";
            }
        }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                if (_selectedDate == value)
                {
                    return;
                }

                Set(ref _selectedDate, value);
                RefreshData();
            }
        }

        private ScheduledToDoPlannerFrame _toDoPlanner;
        public ScheduledToDoPlannerFrame ToDoPlanner
        {
            get
            {
                return _toDoPlanner;
            }
            set
            {
                Set(ref _toDoPlanner, value);
            }
        }

        private ScheduledHabitPlannerFrame _habitPlanner;
        public ScheduledHabitPlannerFrame HabitPlanner
        {
            get
            {
                return _habitPlanner;
            }
            set
            {
                Set(ref _habitPlanner, value);
            }
        }

        private ScheduledWishPlannerFrame _wishPlanner;
        public ScheduledWishPlannerFrame WishPlanner
        {
            get
            {
                return _wishPlanner;
            }
            set
            {
                Set(ref _wishPlanner, value);
            }
        }

        private NotePlannerFrame _notePlanner;
        public NotePlannerFrame NotePlanner
        {
            get
            {
                return _notePlanner;
            }
            set
            {
                Set(ref _notePlanner, value);
            }
        }

        public DailySchedule(int userId)
        {
            _userId = userId == INITIALIZATION_IDENTIFIER ? throw new ArgumentOutOfRangeException(nameof(userId)) : userId;

            NotePlanner = new NotePlannerFrame(_userId);
            NotePlanner.GenerateCommands();

            HabitPlanner = new ScheduledHabitPlannerFrame(_userId, ServiceOperationContract.GET_UNSCHEDULED_HABITS);
            HabitPlanner.GenerateCommands();

            ToDoPlanner = new ScheduledToDoPlannerFrame(_userId, ServiceOperationContract.GET_UNSCHEDULED_TODO_LIST);
            ToDoPlanner.GenerateCommands();

            WishPlanner = new ScheduledWishPlannerFrame(_userId, ServiceOperationContract.GET_UNSCHEDULED_WISH_LIST);
            WishPlanner.GenerateCommands();
        }

        protected override bool InRangeDates(DateTime rangeStartDate, DateTime rangeFinishDate)
        {
            return rangeStartDate.Date <= SelectedDate.Date && rangeFinishDate.Date >= SelectedDate.Date;
        }

        protected override async Task LoadData()
        {
            var requestUri = string.Format(
               ServiceOperationContract.GET_DAILY_SCHEDULE,
               _userId,
               SelectedDate.Date.ToString(ServiceOperationContract.DATE_FORMAT),
               SelectedDate.Date.ToString(ServiceOperationContract.DATE_FORMAT));

            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var planner = await response.Content.ReadAsAsync<Planner>())
                    {
                        ToDoPlanner.Items.OverFill(planner.ToDoList);
                        HabitPlanner.Items.OverFill(planner.Habits);
                        NotePlanner.Items.OverFill(planner.Notes);
                        WishPlanner.Items.OverFill(planner.WishList);
                    }

                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetSchedule.GetDescription(), SelectedDate, response.StatusCode));
            }
        }

        protected override async Task DataSetting()
        {
            NotePlanner.DateRange.StartDate = SelectedDate;
            HabitPlanner.DateRange.StartDate = SelectedDate;
            await HabitPlanner.SetUnscheduledData();
            ToDoPlanner.DateRange.StartDate = SelectedDate;
            await ToDoPlanner.SetUnscheduledData();
            WishPlanner.DateRange.StartDate = SelectedDate;
            await WishPlanner.SetUnscheduledData();
        }
    }
}
