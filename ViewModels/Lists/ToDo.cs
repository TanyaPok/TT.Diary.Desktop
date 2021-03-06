using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class ToDo<T> : AbstractScheduledItem<T>, IPublisher<RefreshData<ToDo<T>>> where T : AbstractScheduleSettings
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.TODO;
            }
        }

        public void Notify(RefreshData<ToDo<T>> message)
        {
            using (var manager = new RefreshDataManager<ToDo<T>>())
            {
                manager.Send(message);
            }
        }

        internal override async Task Remove()
        {
            await base.Remove();
            Notify(new RefreshData<ToDo<T>>());
        }

        internal override async Task Reschedule()
        {
            await base.Reschedule();
            ((IPublisher<RefreshData<ScheduleSettings>>)Schedule).Notify(new RefreshData<ScheduleSettings> { OwnerType = OwnerTypes.ToDo });
            Schedule = null;
        }

        protected override async Task Save()
        {
            await base.Save();

            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<ToDo<T>> { RangeStartDate = Schedule.ScheduledStartDateTime, RangeFinishDate = Schedule.PredictableCompletionDate });
        }

        protected override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.TODO,
                Data = new { Description, CategoryId = ParentId },
                AdditionalInfo = typeof(ToDo<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.TODO,
                Data = new { Id, Description, CategoryId = ParentId },
                AdditionalInfo = typeof(ToDo<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        protected override async Task RemoveTrackers(AbstractScheduledItem<T> owner)
        {
            var requestUri = string.Format(ServiceOperationContract.TODO_TRACKERS, Id);
            await Endpoint.RemoveEntity(new Request { OperationContract = requestUri });
            IsTracked = false;

            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<ToDo<T>> { RangeStartDate = Schedule.ScheduledStartDateTime, RangeFinishDate = Schedule.PredictableCompletionDate });
        }

        internal override async Task Complete()
        {
            await base.Complete();
            ((IPublisher<RefreshData<ScheduleSettings>>)Schedule).Notify(new RefreshData<ScheduleSettings> { OwnerType = OwnerTypes.ToDo });
        }
    }
}
