using System;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public abstract class AbstractScheduleSettings : AbstractEntity
    {
        private readonly int OPERATIONAL_MARGIN = 3;

        public DateTime ScheduledStartDateTime { get; set; }

        private DateTime? _scheduledCompletionDate;
        public DateTime? ScheduledCompletionDate
        {
            get
            {
                return _scheduledCompletionDate;
            }
            set
            {
                ClearErrors(nameof(ScheduledCompletionDate));

                if (value.HasValue && value.Value < ScheduledStartDateTime.Date)
                {
                    AddError(nameof(ScheduledCompletionDate), string.Format(ValidationMessages.IncorrectRange.GetDescription(), "date"));
                }

                Set(ref _scheduledCompletionDate, value);
            }
        }

        private DateTime? _completionDate;
        public DateTime? CompletionDate
        {
            get
            {
                return _completionDate;
            }
            set
            {
                ClearErrors(nameof(CompletionDate));

                if (value.HasValue && value.Value < ScheduledStartDateTime.Date)
                {
                    AddError(nameof(CompletionDate), string.Format(ValidationMessages.IncorrectRange.GetDescription(), "date"));
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

                if (ScheduledCompletionDate.HasValue)
                {
                    return ScheduledCompletionDate.Value;
                }

                return DateTime.MaxValue;
            }
        }

        protected DateTime CalculateCompletionDate()
        {
            if (!ScheduledCompletionDate.HasValue)
            {
                return DateTime.Now;
            }

            if ((DateTime.Now.Date - ScheduledCompletionDate.Value.Date).TotalDays <= OPERATIONAL_MARGIN)
            {
                return ScheduledCompletionDate.Value.Date;
            }

            return DateTime.Now;
        }
    }
}
