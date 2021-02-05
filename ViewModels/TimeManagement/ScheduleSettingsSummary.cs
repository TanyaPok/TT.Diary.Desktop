using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class ScheduleSettingsSummary : AbstractScheduleSettings
    {
        private readonly string LONG_TERM_DATES_FORMAT = "from {0:dd.MM.yyyy} till {1:dd.MM.yyyy}";
        private readonly string SHORT_TERM_DATES_FORMAT = "from {0:dd.MM.yyyy}";

        protected override string RemoveOperationContract => throw new System.NotImplementedException();

        public string TermDatesString
        {
            get
            {
                if (CompletionDate.HasValue)
                {
                    return string.Format(LONG_TERM_DATES_FORMAT, ScheduledStartDateTime, CompletionDate.Value);
                }

                if (ScheduledCompletionDate.HasValue)
                {
                    return string.Format(LONG_TERM_DATES_FORMAT, ScheduledStartDateTime, ScheduledCompletionDate.Value);
                }

                return string.Format(SHORT_TERM_DATES_FORMAT, ScheduledStartDateTime);
            }
        }

        protected override Request GetCreateRequest()
        {
            throw new System.NotImplementedException();
        }

        protected override Request GetUpdateRequest()
        {
            throw new System.NotImplementedException();
        }
    }
}
