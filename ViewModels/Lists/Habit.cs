using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.ScheduleCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Habit<T> : AbstractScheduledItem<T>, IPublisher<RefreshData<Habit<T>>>, ITrackerOwner
        where T : IScheduleSettings
    {
        private uint? _amount;

        [TrackChange]
        public uint? Amount
        {
            get => _amount;
            set
            {
                Set(ref _amount, value);
                RaisePropertyChanged(nameof(HasAmount));
            }
        }

        public bool HasAmount => Amount.HasValue;

        public ICommand AmountValidationCommand { get; private set; }
        public IAttributedCommand TrackersRemoveCommand { get; private set; }

        #region ITrackerOwner

        public Request GetRemoveTrackersRequest()
        {
            return new Request {OperationContract = string.Format(ServiceOperationContract.HabitTrackers, Id)};
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

        public void Notify(RefreshData<Habit<T>> message)
        {
            using (var manager = new RefreshDataManager<Habit<T>>())
            {
                manager.Send(message);
            }
        }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Habit,
                Data = new {Description, CategoryId = ParentId, Amount},
                AdditionalInfo = typeof(Habit<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Habit,
                Data = new {Id, Description, CategoryId = ParentId, Amount},
                AdditionalInfo = typeof(Habit<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        public override Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat, ServiceOperationContract.Habit, Id);
            return new Request {OperationContract = requestUri};
        }

        public override void Notify()
        {
            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<Habit<T>>
            {
                DateRange = new DateRange()
                    {StartDate = Schedule.ScheduledStartDateTime, FinishDate = Schedule.PredictableCompletionDate}
            });
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            AmountValidationCommand = new PatternMatchingCommand("^[0-9]+$");
            TrackersRemoveCommand = new RemoveTrackers<Habit<T>>(this, true);
        }
    }
}