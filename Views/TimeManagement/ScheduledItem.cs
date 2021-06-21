using System.Linq;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.Views.TimeManagement
{
    public static class ScheduledItem
    {
        public static void ConfigureTrackers(AbstractScheduledItem<ScheduleSettings> data, DateRange dateRange,
            MtObservableCollection<ITracker> trackers)
        {
            var start = dateRange.StartDate.Date;
            var finish = dateRange.FinishDate.Date;

            if (start > finish)
            {
                return;
            }

            trackers.Clear();

            do
            {
                var tracker = data.Schedule.Trackers.FirstOrDefault(t => t.ScheduledDate.Date == start);

                if (tracker == null)
                {
                    trackers.Add(new TrackerCap() {ScheduledDate = start});
                }
                else
                {
                    trackers.Add(tracker);
                }

                start = start.AddDays(1);
            } while (finish >= start);
        }
    }
}