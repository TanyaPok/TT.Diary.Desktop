using System.ComponentModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class ToDo<T> : AbstractScheduledItem<T>, IPublisher<DirtyData>, IPublisher<RefreshData<ToDo<T>>> where T : AbstractScheduleSettings
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.TODO;
            }
        }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        public void Notify(RefreshData<ToDo<T>> message)
        {
            using (var manager = new RefreshDataManager<ToDo<T>>())
            {
                manager.Send(message);
            }
        }

        internal override bool CanRemove()
        {
            return Schedule == null;
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
            Notify(new RefreshData<ToDo<T>>());
        }

        internal override Task Reschedule()
        {
            throw new System.NotImplementedException();
        }

        protected override async Task Save()
        {
            await base.Save();
            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
            Notify(new RefreshData<ToDo<T>>());
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

        protected override Task RemoveTrackers(AbstractScheduledItem<T> owner)
        {
            throw new System.NotImplementedException();
        }
    }
}
