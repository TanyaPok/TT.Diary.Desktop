using System;
using System.Linq;
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
        }

        protected override bool InRangeDates(DateTime rangeStartDate, DateTime rangeFinishDate)
        {
            return rangeStartDate.Date <= SelectedDate.Date && rangeFinishDate.Date >= SelectedDate.Date;
        }

        protected override async Task LoadData()
        {
            var requestUri = string.Format(
               ServiceOperationContract.SCHEDULE_REQUEST_FORMAT,
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
                        foreach (var habit in HabitPlanner.Items.ToArray())
                        {
                            HabitPlanner.Items.Remove(habit);
                        }

                        foreach (var habit in planner.Habits)
                        {
                            HabitPlanner.Items.Add(habit);
                        }

                        foreach (var note in NotePlanner.Items.ToArray())
                        {
                            NotePlanner.Items.Remove(note);
                        }

                        foreach (var note in planner.Notes)
                        {
                            NotePlanner.Items.Add(note);
                        }
                    }

                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetSchedule.GetDescription(), SelectedDate, response.StatusCode));
            }
        }

        protected override async Task DataSetting()
        {
            NotePlanner.PlannerDate = SelectedDate;
            HabitPlanner.PlannerDate = SelectedDate;
            await HabitPlanner.SetUnscheduledData();
        }
    }
}
