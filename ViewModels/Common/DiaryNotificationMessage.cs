using GalaSoft.MvvmLight.Messaging;
using System;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public class DiaryNotificationMessage : NotificationMessage
    {
        public DateTime? ScheduledStartDate { get; private set; }
        public DateTime? ScheduledCompletionDate { get; private set; }
        public DateTime? CompletionDate { get; private set; }

        public DiaryNotificationMessage(string notification, DateTime? scheduledStartDate = null, DateTime? scheduledCompletionDate = null, DateTime? completionDate = null) : base(notification)
        {
            ScheduledStartDate = scheduledStartDate;
            ScheduledCompletionDate = scheduledCompletionDate;
            CompletionDate = completionDate;
        }
    }
}
