using GalaSoft.MvvmLight.CommandWpf;
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
    public class Habit<T> : AbstractScheduledItem<T>, IPublisher<RefreshData<Habit<T>>> where T : AbstractScheduleSettings
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

        public void Notify(RefreshData<Habit<T>> message)
        {
            using (var manager = new RefreshDataManager<Habit<T>>())
            {
                manager.Send(message);
            }
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            AmountValidationCommand = new RelayCommand<TextCompositionEventArgs>(AmountValidation, canExecute: e => true);
        }

        internal override async Task Remove()
        {
            await base.Remove();
            Notify(new RefreshData<Habit<T>>());
        }

        internal override async Task Reschedule()
        {
            await base.Reschedule();
            ((IPublisher<RefreshData<ScheduleSettings>>)Schedule).Notify(new RefreshData<ScheduleSettings> { OwnerType = OwnerTypes.Habit });
            Schedule = null;
        }

        internal override async Task Complete()
        {
            await base.Complete();
            ((IPublisher<RefreshData<ScheduleSettings>>)Schedule).Notify(new RefreshData<ScheduleSettings> { OwnerType = OwnerTypes.Habit });
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

            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<Habit<T>> { RangeStartDate = Schedule.ScheduledStartDateTime, RangeFinishDate = Schedule.PredictableCompletionDate });
        }

        protected override async Task RemoveTrackers(AbstractScheduledItem<T> owner)
        {
            var requestUri = string.Format(ServiceOperationContract.HABIT_TRACKERS, Id);
            await Endpoint.RemoveEntity(new Request { OperationContract = requestUri });
            IsTracked = false;

            if (Schedule == null)
            {
                return;
            }

            Notify(new RefreshData<Habit<T>> { RangeStartDate = Schedule.ScheduledStartDateTime, RangeFinishDate = Schedule.PredictableCompletionDate });
        }

        private void AmountValidation(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
