using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.Notification
{
    public enum OwnerTypes
    {
        Unknown,
        ToDo,
        Appointment,
        Habit,
        Wish
    }

    /// <summary>
    /// Notification message for reload data. The message will be dispose after processing.
    /// </summary>
    public class RefreshData<T> : IMessage where T : AbstractEntity
    {
        public OwnerTypes OwnerType { get; internal set; }

        public DateRange DateRange { get; internal set; }
    }
}