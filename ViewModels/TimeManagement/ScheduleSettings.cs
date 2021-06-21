using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.RemoveCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
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

    public class ScheduleSettings : AbstractEntity, IScheduleSettings, IPublisher<RefreshData<ScheduleSettings>>,
        IRemovable
    {
        private readonly int _operationalMargin = 3;
        internal string OperationContract { get; set; }
        internal OwnerTypes OwnerType { get; set; }

        #region IScheduleSettings

        private DateTime _scheduledStartDateTime;

        [TrackChange]
        public DateTime ScheduledStartDateTime
        {
            get => _scheduledStartDateTime;
            set => Set(ref _scheduledStartDateTime, value);
        }

        private DateTime? _scheduledCompletionDate;

        [TrackChange]
        public DateTime? ScheduledCompletionDate
        {
            get => _scheduledCompletionDate;
            set
            {
                ClearErrors(nameof(ScheduledCompletionDate));

                if (!CheckDateRange(value))
                {
                    AddError(nameof(ScheduledCompletionDate),
                        string.Format(ValidationMessages.IncorrectRange.GetDescription(), "date"));
                }

                Set(ref _scheduledCompletionDate, value);
            }
        }

        private DateTime? _completionDate;

        [TrackChange]
        public DateTime? CompletionDate
        {
            get => _completionDate;
            set
            {
                ClearErrors(nameof(CompletionDate));

                if (!CheckDateRange(value))
                {
                    AddError(nameof(CompletionDate),
                        string.Format(ValidationMessages.IncorrectRange.GetDescription(), "date"));
                }

                Set(ref _completionDate, value);
            }
        }

        public DateTime PredictableCompletionDate
        {
            get
            {
                if (CompletionDate.HasValue)
                {
                    return CompletionDate.Value;
                }

                return ScheduledCompletionDate ?? DateTime.MaxValue;
            }
        }

        #endregion

        #region public properties

        public DateTime? LastCompletedTrackerDate { get; set; }

        private Repeat _repeat;

        [TrackChange]
        public Repeat Repeat
        {
            get => _repeat;
            set
            {
                Set(ref _repeat, value);
                Verify(nameof(Every));
                Verify(nameof(Weekdays));
                Verify(nameof(Months));

                if (Repeat.None != value) return;
                ScheduledCompletionDate = ScheduledStartDateTime.Date.AddMinutes(1439);
            }
        }

        private uint? _every;

        [TrackChange]
        public uint? Every
        {
            get => _every;
            set
            {
                Set(ref _every, value ?? 0);
                Verify(nameof(Every));
            }
        }

        private Months _months;

        [TrackChange]
        public Months Months
        {
            get => _months;
            set
            {
                Set(ref _months, value);
                Verify(nameof(Months));
            }
        }

        private Weekdays _weekdays;

        [TrackChange]
        public Weekdays Weekdays
        {
            get => _weekdays;
            set
            {
                Set(ref _weekdays, value);
                Verify(nameof(Weekdays));
            }
        }

        private uint? _daysAmount;

        [TrackChange]
        public uint? DaysAmount
        {
            get => _daysAmount;
            set => Set(ref _daysAmount, value ?? 0);
        }

        #endregion

        public ObservableCollection<Tracker> Trackers { get; }
        public ICommand EveryValidationCommand { get; private set; }

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

        public void Notify(RefreshData<ScheduleSettings> message)
        {
            using (var manager = new RefreshDataManager<ScheduleSettings>())
            {
                manager.Send(message);
            }
        }

        public override Request GetCreateRequest()
        {
            return new Request()
            {
                OperationContract = OperationContract,
                Data = new
                {
                    OwnerId = ParentId, ScheduledStartDateTime, ScheduledCompletionDate, CompletionDate, Repeat,
                    Every = Every ?? 0, Months, Weekdays, DaysAmount = DaysAmount ?? 0
                },
                AdditionalInfo = nameof(ScheduleSettings)
            };
        }

        public override Request GetUpdateRequest()
        {
            return GetCreateRequest();
        }

        public override void AfterAcceptChanges()
        {
            base.AfterAcceptChanges();
            Notify();
        }

        #region implement IRemovable

        public bool CanBeRemoved()
        {
            return true;
        }

        public Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat, OperationContract, Id);
            return new Request {OperationContract = requestUri};
        }

        #endregion

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            EveryValidationCommand = new PatternMatchingCommand("^[0-9]+$");
        }

        internal DateTime CalculateCompletionDate()
        {
            if (LastCompletedTrackerDate.HasValue && LastCompletedTrackerDate.Value > DateTime.MinValue)
            {
                return LastCompletedTrackerDate.Value;
            }

            if (!ScheduledCompletionDate.HasValue)
            {
                return DateTime.Now;
            }

            var daysCount = (DateTime.Now.Date - ScheduledCompletionDate.Value.Date).TotalDays;

            if (daysCount <= _operationalMargin && daysCount > 0)
            {
                return ScheduledCompletionDate.Value.Date;
            }

            return DateTime.Now;
        }

        internal bool CheckDateRange(DateTime? value)
        {
            return !(value.HasValue && value.Value < ScheduledStartDateTime.Date);
        }

        internal void Notify()
        {
            Notify(new RefreshData<ScheduleSettings>()
            {
                DateRange = new DateRange {StartDate = ScheduledStartDateTime, FinishDate = PredictableCompletionDate},
                OwnerType = OwnerType
            });
        }

        private static void Trackers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var tracker = (Tracker) item;
                        tracker.GenerateCommands();
                        tracker.SubscribeToPropertyChanging();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var tracker = (Tracker) item;
                        tracker.Dispose();
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        private void Verify(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Every):
                    ClearErrors(nameof(Every));

                    if ((!Every.HasValue || Every.Value == 0) && Repeat != Repeat.None)
                    {
                        AddError(nameof(Every), ValidationMessages.IncorrectGap.GetDescription());
                    }

                    break;
                case nameof(Months):
                    ClearErrors(nameof(Months));

                    if (Months == 0 && Repeat == Repeat.Yearly)
                    {
                        AddError(nameof(Months),
                            string.Format(ValidationMessages.IncorrectRange.GetDescription(), nameof(Months)));
                    }

                    break;
                case nameof(Weekdays):
                    ClearErrors(nameof(Weekdays));

                    if (Weekdays == 0 && Repeat == Repeat.Weekly)
                    {
                        AddError(nameof(Weekdays),
                            string.Format(ValidationMessages.IncorrectRange.GetDescription(), nameof(Weekdays)));
                    }

                    break;
            }
        }
    }
}