using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class Tracker : AbstractEntity, IPublisher<DirtyData>
    {
        private readonly int OPERATIONAL_MARGIN = 3;

        protected override string RemoveOperationContract
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal string OperationContract { get; set; }

        private DateTime _scheduledDate;
        public DateTime ScheduledDate
        {
            get
            {
                return _scheduledDate;
            }
            set
            {
                Set(ref _scheduledDate, value);
            }
        }

        public DateTime? DateTime { set; get; }

        private double _progress;
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                Set(ref _progress, value);
            }
        }

        private double? _value;
        public double? Value
        {
            get
            {
                return _value;
            }
            set
            {
                Set(ref _value, value);
            }
        }

        public ICommand AmountValidationCommand { get; private set; }

        public ICommand ValueSaveCommand { get; private set; }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        public override bool CanAcceptChanges()
        {
            return base.CanAcceptChanges() && ParentId > INITIALIZATION_IDENTIFIER;
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            AmountValidationCommand = new RelayCommand<TextCompositionEventArgs>(AmountValidation, canExecute: e => true);
            ValueSaveCommand = new RelayCommand(async () => { await ValueSave(); }, () => true, true);
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
        }

        protected override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = this.OperationContract,
                Data = new { OwnerId = ParentId, ScheduledDate, DateTime, Progress, Value },
                AdditionalInfo = nameof(Tracker) + " on " + ScheduledDate
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = this.OperationContract,
                Data = new { Id, DateTime, Progress, Value },
                AdditionalInfo = nameof(Tracker) + " on " + ScheduledDate
            };
        }

        protected override async Task Save()
        {
            await base.Save();
            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Id))
            {
                return;
            }

            DateTime = CalculateDateTime();
            Notify(new DirtyData { Source = this, Operation = OperationType.Add });
        }

        private DateTime CalculateDateTime()
        {
            if ((System.DateTime.Now.Date - ScheduledDate.Date).TotalDays <= OPERATIONAL_MARGIN)
            {
                return ScheduledDate.Date;
            }

            return System.DateTime.Now;
        }

        private void AmountValidation(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private async Task ValueSave()
        {
            if (CanAcceptChanges())
            {
                await AcceptChanges();
            }
        }
    }
}
