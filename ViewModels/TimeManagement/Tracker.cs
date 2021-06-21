using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class Tracker : AbstractEntity, ITracker, IPublisher<RefreshData<Tracker>>
    {
        private readonly int _operationalMargin = 3;
        internal string OperationContract { get; set; }
        internal OwnerTypes OwnerType { get; set; }

        private DateTime _scheduledDate;

        public DateTime ScheduledDate
        {
            get => _scheduledDate;
            set => Set(ref _scheduledDate, value);
        }

        public DateTime? DateTime { set; get; }

        private double _progress;

        [TrackChange]
        public double Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
        }

        private double? _value;

        [TrackChange]
        public double? Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public ICommand AmountValidationCommand { get; private set; }

        public ICommand ValueSaveCommand { get; private set; }

        // public override bool CanAcceptChanges()
        // {
        //     return base.CanAcceptChanges() && ParentId > default(int);
        // }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = this.OperationContract,
                Data = new {OwnerId = ParentId, ScheduledDate, DateTime, Progress, Value},
                AdditionalInfo = nameof(Tracker) + " on " + ScheduledDate
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = this.OperationContract,
                Data = new {Id, DateTime, Progress, Value},
                AdditionalInfo = nameof(Tracker) + " on " + ScheduledDate
            };
        }

        public override void AfterAcceptChanges()
        {
            base.AfterAcceptChanges();
            Notify(new RefreshData<Tracker>()
            {
                DateRange =
                    new DateRange {StartDate = ScheduledDate, FinishDate = DateTime ?? System.DateTime.MaxValue},
                OwnerType = OwnerType
            });
        }

        public void Notify(RefreshData<Tracker> message)
        {
            using (var manager = new RefreshDataManager<Tracker>())
            {
                manager.Send(message);
            }
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            AmountValidationCommand = new PatternMatchingCommand("^[0-9]+$");
            ValueSaveCommand = new RelayCommand(async () => { await ValueSave(); }, () => true, true);
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);
            DateTime = CalculateDateTime();
        }

        private DateTime CalculateDateTime()
        {
            return (System.DateTime.Now.Date - ScheduledDate.Date).TotalDays <= _operationalMargin
                ? ScheduledDate.Date
                : System.DateTime.Now;
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