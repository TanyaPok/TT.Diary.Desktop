using System;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Note : AbstractListItem, IPublisher<RefreshData<Note>>
    {
        private DateTime _previousScheduledDate = DateTime.Now;
        private DateTime _scheduleDate = DateTime.Now;

        [TrackChange]
        public DateTime ScheduleDate
        {
            get => _scheduleDate;
            set
            {
                _previousScheduledDate = _scheduleDate;
                Set(ref _scheduleDate, value);
            }
        }

        public void Notify(RefreshData<Note> message)
        {
            using (var messenger = new RefreshDataManager<Note>())
            {
                messenger.Send(message);
            }
        }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Note,
                Data = new {Description, ScheduleDate, UserId},
                AdditionalInfo = "Note " + Description
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Note,
                Data = new {Id, Description, ScheduleDate, UserId},
                AdditionalInfo = "Note " + Description
            };
        }

        public override bool CanBeRemoved()
        {
            return true;
        }

        public override Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat, ServiceOperationContract.Note, Id);
            return new Request {OperationContract = requestUri};
        }

        public override void Notify()
        {
            Notify(new RefreshData<Note>
            {
                DateRange = new DateRange()
                    {StartDate = ScheduleDate, FinishDate = ScheduleDate}
            });


            if (_previousScheduledDate.Date != _scheduleDate.Date)
            {
                Notify(new RefreshData<Note>
                {
                    DateRange = new DateRange()
                        {StartDate = _previousScheduledDate, FinishDate = _previousScheduledDate}
                });
            }
        }
    }
}