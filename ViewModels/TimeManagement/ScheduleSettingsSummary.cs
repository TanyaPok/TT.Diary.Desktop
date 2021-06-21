using System;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class ScheduleSettingsSummary : IScheduleSettings
    {
        private readonly string _longTermDatesFormat = "from {0:dd.MM.yyyy} till {1:dd.MM.yyyy}";
        private readonly string _shortTermDatesFormat = "from {0:dd.MM.yyyy}";

        public string TermDatesString
        {
            get
            {
                if (CompletionDate.HasValue)
                {
                    return string.Format(_longTermDatesFormat, ScheduledStartDateTime, CompletionDate.Value);
                }

                return ScheduledCompletionDate.HasValue
                    ? string.Format(_longTermDatesFormat, ScheduledStartDateTime, ScheduledCompletionDate.Value)
                    : string.Format(_shortTermDatesFormat, ScheduledStartDateTime);
            }
        }

        public int Id { get; set; }
        public DateTime ScheduledStartDateTime { get; set; }
        public DateTime? ScheduledCompletionDate { get; set; }
        public DateTime? CompletionDate { get; set; }

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
    }
}