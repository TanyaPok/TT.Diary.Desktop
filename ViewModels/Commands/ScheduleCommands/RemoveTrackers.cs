using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Commands.ScheduleCommands
{
    public class RemoveTrackers<T> : RelayCommand, IAttributedCommand where T : ITrackerOwner
    {
        public string Name => $"Remove trackers";
        public string ImgUrl => "pack://application:,,,/Images/Toolbar/remove.png";

        public RemoveTrackers(T owner, bool keepTargetAlive = false)
            : base
            (
                async () =>
                {
                    await Endpoint.RemoveAsync(owner.GetRemoveTrackersRequest());
                    owner.AfterRemovingTrackers();
                },
                owner.CanRemoveTrackers,
                keepTargetAlive
            )
        {
        }
    }
}