using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Note : AbstractListItem, IPublisher<DirtyData>, IPublisher<RefreshData<Note>>
    {
        private DateTime _previousScheduledDate;

        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.NOTE;
            }
        }

        private DateTime _scheduleDate = DateTime.Now;
        public DateTime ScheduleDate
        {
            get
            {
                return _scheduleDate;
            }
            set
            {
                _previousScheduledDate = _scheduleDate;
                Set(ref _scheduleDate, value);
            }
        }

        public void Notify(RefreshData<Note> message)
        {
            using (var messager = new RefreshDataManager<Note>())
            {
                messager.Send(message);
            }
        }

        public void Notify(DirtyData message)
        {
            using (var messager = new DirtyDataManager())
            {
                messager.Send(message);
            }
        }

        internal override async Task Remove()
        {
            if (State == EntityState.New)
            {
                Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
                return;
            }

            await base.Remove();
            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
            Notify(new RefreshData<Note> { RangeStartDate = ScheduleDate, RangeFinishDate = ScheduleDate });
            Notify(new RefreshData<Note> { RangeStartDate = _previousScheduledDate, RangeFinishDate = _previousScheduledDate });
        }

        protected override async Task Save()
        {
            await base.Save();
            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
            Notify(new RefreshData<Note> { RangeStartDate = ScheduleDate, RangeFinishDate = ScheduleDate });
            Notify(new RefreshData<Note> { RangeStartDate = _previousScheduledDate, RangeFinishDate = _previousScheduledDate });
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);
            Notify(new DirtyData { Source = this, Operation = OperationType.Add });
        }

        protected override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.NOTE,
                Data = new { Description, ScheduleDate, UserId },
                AdditionalInfo = "Note " + Description
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.NOTE,
                Data = new { Id, Description, ScheduleDate, UserId },
                AdditionalInfo = "Note " + Description
            };
        }
    }
}
