using GalaSoft.MvvmLight.CommandWpf;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Habit<T> : AbstractScheduledItem<T>, IPublisher<DirtyData>, IPublisher<RefreshData<Habit<T>>> where T : AbstractScheduleSettings
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.HABIT;
            }
        }

        private uint? _amount;
        public uint? Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                Set(ref _amount, value);
                RaisePropertyChanged(nameof(HasAmount));
            }
        }

        public bool HasAmount
        {
            get
            {
                return Amount.HasValue;
            }
        }

        public ICommand AmountValidationCommand { get; private set; }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        public void Notify(RefreshData<Habit<T>> message)
        {
            using (var manager = new RefreshDataManager<Habit<T>>())
            {
                manager.Send(message);
            }
        }

        public override bool CanAcceptChanges()
        {
            return base.CanAcceptChanges() && ParentId != INITIALIZATION_IDENTIFIER;
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            AmountValidationCommand = new RelayCommand<TextCompositionEventArgs>(AmountValidation, canExecute: e => true);
        }

        internal override bool CanRemove()
        {
            return Schedule == null || Schedule.State == EntityState.New;
        }

        internal override async Task Remove()
        {
            if (Schedule != null && Schedule.State == EntityState.New && Schedule.CanRemove())
            {
                await Schedule.Remove();
                Schedule = null;
            }

            if (State == EntityState.New)
            {
                Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
                return;
            }

            await base.Remove();

            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
            Notify(new RefreshData<Habit<T>>());
        }

        internal override async Task Reschedule()
        {
            if (!Schedule.CanRemove())
            {
                return;
            }

            await Schedule.Remove();
            ((IPublisher<RefreshData<ScheduleSettings>>)Schedule).Notify(new RefreshData<ScheduleSettings> { OwnerType = OwnerTypes.Habit });
            Schedule = null;
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTracked))
            {
                return;
            }

            base.EntityPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Schedule) && Schedule == null)
            {
                return;
            }

            Notify(new DirtyData { Source = this, Operation = OperationType.Add });
        }

        protected override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.HABIT,
                Data = new { Description, CategoryId = ParentId, Amount },
                AdditionalInfo = typeof(Habit<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.HABIT,
                Data = new { Id, Description, CategoryId = ParentId, Amount },
                AdditionalInfo = typeof(Habit<T>).GetNameWithoutGenericArity() + " " + Description
            };
        }

        protected override async Task Save()
        {
            await base.Save();

            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });

            if (Schedule == null)
            {
                return;
            }

            Schedule.ParentId = Id;

            if (Schedule.CanAcceptChanges())
            {
                await Schedule.AcceptChanges();
            }

            Notify(new RefreshData<Habit<T>> { RangeStartDate = Schedule.ScheduledStartDateTime, RangeFinishDate = Schedule.PredictableCompletionDate });
        }

        protected override async Task RemoveTrackers(AbstractScheduledItem<T> owner)
        {
            var requestUri = string.Format(ServiceOperationContract.REQUEST_FORMAT, ServiceOperationContract.HABIT_TRACKERS, Id);
            await Endpoint.RemoveEntity(new Request { OperationContract = requestUri });
            IsTracked = false;
            Notify(new RefreshData<Habit<T>> { RangeStartDate = Schedule.ScheduledStartDateTime, RangeFinishDate = Schedule.PredictableCompletionDate });
        }

        private void AmountValidation(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
