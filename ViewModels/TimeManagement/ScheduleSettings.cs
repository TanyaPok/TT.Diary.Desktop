using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public enum Repeat
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    [Flags]
    public enum Weekdays
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }

    [Flags]
    public enum Months
    {
        January = 1,
        February = 2,
        March = 4,
        April = 8,
        May = 16,
        June = 32,
        July = 64,
        August = 128,
        September = 256,
        October = 512,
        November = 1024,
        December = 2048
    }

    public class ScheduleSettings : AbstractScheduleSettings, IPublisher<DirtyData>, IPublisher<RefreshData<ScheduleSettings>>
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return OperationContract;
            }
        }

        internal string OperationContract { get; set; }

        private Repeat _repeat;
        public Repeat Repeat
        {
            get
            {
                return _repeat;
            }
            set
            {
                Set(ref _repeat, value);
                Every = Every;
            }
        }

        private uint _every;
        public uint Every
        {
            get
            {
                return _every;
            }
            set
            {
                ClearErrors(nameof(Every));

                if (value < 1 && Repeat != Repeat.None)
                {
                    AddError(nameof(Every), ValidationMessages.IncorrectGap.GetDescription());
                }

                Set(ref _every, value);
            }
        }

        private Months _months;
        public Months Months
        {
            get
            {
                return _months;
            }
            set
            {
                ClearErrors(nameof(Months));

                if (value == 0 && Repeat == Repeat.Yearly)
                {
                    AddError(nameof(Months), string.Format(ValidationMessages.IncorrectRange.GetDescription(), nameof(Months)));
                }

                Set(ref _months, value);
            }
        }

        private Weekdays _weekdays;
        public Weekdays Weekdays
        {
            get
            {
                return _weekdays;
            }
            set
            {
                ClearErrors(nameof(Weekdays));

                if (value == 0 && Repeat == Repeat.Weekly)
                {
                    AddError(nameof(Weekdays), string.Format(ValidationMessages.IncorrectRange.GetDescription(), nameof(Weekdays)));
                }

                Set(ref _weekdays, value);
            }

        }

        private uint _daysAmount;
        public uint DaysAmount
        {
            get
            {
                return _daysAmount;
            }
            set
            {
                Set(ref _daysAmount, value);
            }
        }

        public IAttributedCommand CompleteCommand { get; protected set; }

        public ObservableCollection<Tracker> Trackers { get; private set; }

        public ScheduleSettings()
        {
            Trackers = new ObservableCollection<Tracker>();
            Trackers.CollectionChanged += Trackers_CollectionChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            Trackers.CollectionChanged -= Trackers_CollectionChanged;
        }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        public void Notify(RefreshData<ScheduleSettings> message)
        {
            using (var manager = new RefreshDataManager<ScheduleSettings>())
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
            CompleteCommand = new CompleteCommand(async () => { await Complete(); }, CanComplete, true);
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

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);
            Notify(new DirtyData { Source = this, Operation = OperationType.Add });
        }

        protected override async Task Save()
        {
            Id = await Endpoint.CreateEntity(GetCreateRequest());

            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });

            foreach (var tracker in Trackers)
            {
                tracker.ParentId = ParentId;

                if (tracker.CanAcceptChanges())
                {
                    await tracker.AcceptChanges();
                }
            }
        }

        protected override Request GetCreateRequest()
        {
            return new Request()
            {
                OperationContract = OperationContract,
                Data = new { OwnerId = ParentId, ScheduledStartDateTime, ScheduledCompletionDate, CompletionDate, Repeat, Every, Months, Weekdays, DaysAmount },
                AdditionalInfo = nameof(ScheduleSettings)
            };
        }

        protected override Request GetUpdateRequest()
        {
            throw new NotImplementedException();
        }

        private void Trackers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var tracker = (Tracker)item;
                        tracker.GenerateCommands();
                        tracker.SubscribeToPropertyChanging();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var tracker = (Tracker)item;
                        tracker.Dispose();
                    }
                    break;
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        private bool CanComplete()
        {
            return !CompletionDate.HasValue;
        }

        private async Task Complete()
        {
            CompletionDate = CalculateCompletionDate();

            if (CanAcceptChanges())
            {
                await AcceptChanges();
            }
        }
    }
}
