using GalaSoft.MvvmLight.Messaging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class DailySchedule : ContentControlViewModel
    {
        private int _userId;

        public string Title
        {
            get
            {
                return "Daily schedule for";
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                var oldValue = _selectedDate;
                Set(ref _selectedDate, value);

                if (oldValue != value && IsDataLoaded)
                {
                    IsDataLoaded = false;
                    Initialize(_userId);
                }
            }
        }

        private Planner _planner;
        public Planner Planner 
        {
            get
            {
                return _planner;
            }
            set
            {
                Set(ref _planner, value);
            }
        }

        public DailySchedule()
        {
            SelectedDate = DateTime.Now;

            Messenger.Default.Register<DiaryNotificationMessage>(
               this,
               message =>
               {
                   if (message.Notification == LISTVIEWMODEL_NOTE && SelectedDate.Date == message.ScheduledStartDate.Value.Date)
                   {
                       IsDataLoaded = false;
                       return;
                   }
               });
        }

        protected override async Task LoadData(int userId)
        {
            _userId = userId;
            var requestUri = string.Format(
               OperationContract.SCHEDULE_REQUEST_FORMAT,
               OperationContract.GET_DAILY_SCHEDULE,
               _userId,
               SelectedDate.Date.ToString(DATE_FORMAT),
               SelectedDate.Date.ToString(DATE_FORMAT));

            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    Planner = await response.Content.ReadAsAsync<Planner>();
                    Planner.SenderPath = nameof(DailySchedule);
                    Planner.StartDate = SelectedDate;
                    Planner.FinishDate = SelectedDate;
                    Planner.UserId = _userId;
                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetSchedule.GetDescription(), SelectedDate, response.StatusCode));
            }
        }
    }
}
