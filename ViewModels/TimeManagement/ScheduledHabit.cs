using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class ScheduledHabit : AbstractHabit<ScheduleSettings>
    {
        public override void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager<ScheduledHabit>())
            {
                manager.Send(message);
            }
        }
    }
}
