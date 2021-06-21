using System;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public interface IScheduleSettings
    {
        int Id { get; }
        DateTime ScheduledStartDateTime { get; }
        DateTime? ScheduledCompletionDate { get; }
        DateTime? CompletionDate { get; }
        DateTime PredictableCompletionDate { get; }
    }
}