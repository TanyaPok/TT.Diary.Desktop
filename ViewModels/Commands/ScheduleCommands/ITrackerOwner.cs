using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Commands.ScheduleCommands
{
    public interface ITrackerOwner
    {
        Request GetRemoveTrackersRequest();
        bool CanRemoveTrackers();
        void AfterRemovingTrackers();
    }
}