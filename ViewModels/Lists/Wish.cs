using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
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

    public class Wish<T> : AbstractScheduledItem<T>, IPublisher<RefreshData<Wish<T>>> where T : IScheduleSettings
    {
        private Rating _rating;

        [TrackChange]
        public Rating Rating
        {
            set => Set(ref _rating, value);
            get => _rating;
        }

        private string _author;

        [TrackChange]
        public string Author
        {
            set => Set(ref _author, value);
            get => _author;
        }

        public void Notify(RefreshData<Wish<T>> message)
        {
            using (var manager = new RefreshDataManager<Wish<T>>())
            {
                manager.Send(message);
            }
        }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Wish,
                Data = new {Description, CategoryId = ParentId, Author, Rating},
                AdditionalInfo = typeof(Wish<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Wish,
                Data = new {Id, Description, CategoryId = ParentId, Author, Rating},
                AdditionalInfo = typeof(Wish<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat, ServiceOperationContract.Wish, Id);
            return new Request {OperationContract = requestUri};
        }

        public override void Notify()
        {
            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<Wish<T>>
            {
                DateRange = new DateRange()
                    {StartDate = Schedule.ScheduledStartDateTime, FinishDate = Schedule.PredictableCompletionDate}
            });
        }
    }
}