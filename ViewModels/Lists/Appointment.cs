using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.ScheduleCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Appointment<T> : AbstractScheduledItem<T>, IPublisher<RefreshData<Appointment<T>>>, ITrackerOwner
        where T : IScheduleSettings
    {
        public IAttributedCommand TrackersRemoveCommand { get; private set; }

        #region ITrackerOwner

        public Request GetRemoveTrackersRequest()
        {
            return new Request {OperationContract = string.Format(ServiceOperationContract.AppointmentTrackers, Id)};
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

        public void Notify(RefreshData<Appointment<T>> message)
        {
            using (var manager = new RefreshDataManager<Appointment<T>>())
            {
                manager.Send(message);
            }
        }

        public override void Notify()
        {
            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<Appointment<T>>
            {
                DateRange = new DateRange()
                    {StartDate = Schedule.ScheduledStartDateTime, FinishDate = Schedule.PredictableCompletionDate}
            });
        }

        public override Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat,
                ServiceOperationContract.Appointment, Id);
            return new Request {OperationContract = requestUri};
        }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Appointment,
                Data = new {Description, CategoryId = ParentId},
                AdditionalInfo = typeof(Appointment<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Appointment,
                Data = new {Id, Description, CategoryId = ParentId},
                AdditionalInfo = typeof(Appointment<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            TrackersRemoveCommand = new RemoveTrackers<Appointment<T>>(this, true);
        }
    }
}