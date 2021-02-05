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
    public enum Rating
    {
        Empty,
        Trash,
        NotSoBad,
        Normal,
        Good,
        Fire
    }

    public class Wish<T> : AbstractScheduledItem<T>, IPublisher<DirtyData>, IPublisher<RefreshData<Wish<T>>> where T : AbstractScheduleSettings
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.WISH;
            }
        }

        private Rating _rating;
        public Rating Rating
        {
            set
            {
                Set(ref _rating, value);
            }
            get
            {
                return _rating;
            }
        }

        private string _author;
        public string Author
        {
            set
            {
                Set(ref _author, value);
            }
            get
            {
                return _author;
            }
        }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        public void Notify(RefreshData<Wish<T>> message)
        {
            using (var manager = new RefreshDataManager<Wish<T>>())
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
            Notify(new RefreshData<Wish<T>>());
        }

        internal override Task Reschedule()
        {
            throw new System.NotImplementedException();
        }

        protected override async Task Save()
        {
            await base.Save();
            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
            Notify(new RefreshData<Wish<T>>());
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
                OperationContract = ServiceOperationContract.WISH,
                Data = new { Description, CategoryId = ParentId, Author, Rating },
                AdditionalInfo = typeof(Wish<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.WISH,
                Data = new { Id, Description, CategoryId = ParentId, Author, Rating },
                AdditionalInfo = typeof(Wish<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        protected override Task RemoveTrackers(AbstractScheduledItem<T> owner)
        {
            throw new System.NotImplementedException();
        }
    }
}