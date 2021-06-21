using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class
        ScheduledWishPlannerFrame : AbstractScheduledItemPlannerFrame<Wish<ScheduleSettings>, UnscheduledWishSummary>
    {
        public ScheduledWishPlannerFrame(int userId) : base(userId,
            ServiceOperationContract.GetUnscheduledWishList, ServiceOperationContract.WishSchedule)
        {
        }

        protected override void FillUnscheduledItemSummaries(IEnumerable<Wish<ScheduleSettings>> data)
        {
            foreach (var item in data)
            {
                UnscheduledItemSummaries.Add(
                    new UnscheduledWishSummary()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Author = item.Author,
                        Rating = item.Rating,
                        ParentId = item.ParentId
                    });
            }
        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var wish = new Wish<ScheduleSettings>();

            var schedule = new ScheduleSettings();
            wish.Schedule = schedule;

            Items.Add(wish);

            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.Repeat = Repeat.Daily;
            schedule.Every = 1;
        }

        protected override void TemplateChange(SelectionChangedEventArgs e)
        {
            base.TemplateChange(e);

            if (NewItem == null || Template == null)
            {
                return;
            }

            NewItem.Author = Template.Author;
            NewItem.Rating = Template.Rating;
        }
    }
}