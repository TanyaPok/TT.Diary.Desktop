using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.ScheduleCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class ToDo<T> : AbstractScheduledItem<T>, IPublisher<RefreshData<ToDo<T>>>, ITrackerOwner
        where T : IScheduleSettings
    {
        public IAttributedCommand TrackersRemoveCommand { get; private set; }

        #region ITrackerOwner

        public Request GetRemoveTrackersRequest()
        {
            return new Request {OperationContract = string.Format(ServiceOperationContract.TodoTrackers, Id)};
        }

        public bool CanRemoveTrackers()
        {
            return IsTracked;
        }

        public void AfterRemovingTrackers()
        {
            IsTracked = false;
            TrackersRemoveCommand.RaiseCanExecuteChanged();
            Notify();
        }

        #endregion

        public void Notify(RefreshData<ToDo<T>> message)
        {
            using (var manager = new RefreshDataManager<ToDo<T>>())
            {
                manager.Send(message);
            }
        }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Todo,
                Data = new {Description, CategoryId = ParentId},
                AdditionalInfo = typeof(ToDo<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Todo,
                Data = new {Id, Description, CategoryId = ParentId},
                AdditionalInfo = typeof(ToDo<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat, ServiceOperationContract.Todo, Id);
            return new Request {OperationContract = requestUri};
        }

        public override void Notify()
        {
            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<ToDo<T>>
            {
                DateRange = new DateRange()
                    {StartDate = Schedule.ScheduledStartDateTime, FinishDate = Schedule.PredictableCompletionDate}
            });
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            TrackersRemoveCommand = new RemoveTrackers<ToDo<T>>(this, true);
        }
    }
}