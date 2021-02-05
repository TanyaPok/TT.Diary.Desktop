using GalaSoft.MvvmLight.CommandWpf;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractHabit<T> : AbstractListItem, IPublisher<DirtyData>, IPublisher<RefreshData> where T : AbstractScheduleSettings
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.HABIT;
            }
        }

        public T Schedule { get; set; }

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
            }
        }

        public ICommand AmountValidationCommand { get; private set; }

        public AbstractHabit()
        {
            AmountValidationCommand = new RelayCommand<TextCompositionEventArgs>(AmountValidation, canExecute: e => true);
        }

        public abstract void Notify(DirtyData message);

        public void Notify(RefreshData message)
        {
            using (var manager = new RefreshDataManager())
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
            if (Id == 0)
            {
                Notify(new DirtyData { SenderPath = SenderPath, Entiry = this, Operation = OperationType.Remove });
                return;
            }

            await base.Remove();

            Notify(new DirtyData { SenderPath = SenderPath, Entiry = this, Operation = OperationType.Remove });
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);
            Notify(new DirtyData { SenderPath = SenderPath, Entiry = this, Operation = OperationType.Add });
        }

        protected override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.HABIT,
                Data = new { Description, CategoryId = ParentId, Amount },
                AdditionalInfo = "Habit " + Description
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.HABIT,
                Data = new { Id, Description, CategoryId = ParentId, Amount },
                AdditionalInfo = "Habit " + Description
            };
        }

        private void AmountValidation(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
