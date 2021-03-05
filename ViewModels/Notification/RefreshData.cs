using System;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

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
    /// Notification message for reload data. The message will be dispose after processings.
    /// </summary>
    public class RefreshData<T> : IMessage where T : AbstractEntity
    {
        public OwnerTypes OwnerType { get; internal set; }

        public DateTime RangeStartDate { get; internal set; }

        public DateTime RangeFinishDate { get; internal set; }
    }
}
