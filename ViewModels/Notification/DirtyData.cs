using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.Notification
{
    public enum OperationType
    {
        Add,
        Remove
    }

    /// <summary>
    /// Notification message for form dirty data that must be saved. The message will be disposed after processing.
    /// </summary>
    public class DirtyData : IMessage
    {
        internal AbstractEntity Source { get; set; }

        public OperationType Operation { get; internal set; }
    }
}