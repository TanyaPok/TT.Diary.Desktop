using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class ScheduledWishPlannerFrame : AbstractScheduledItemPlannerFrame<Wish<ScheduleSettings>, UnscheduledWishSummary>
    {
        public ScheduledWishPlannerFrame(int userId, string getUnscheduledItemsOperation)
            : base(userId, getUnscheduledItemsOperation, ServiceOperationContract.WISH_SCHEDULE)
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
                        ParentId = item.ParentId
                    });
            }
        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var newWish = new Wish<ScheduleSettings>
            {
                PreSave = PreSaveItem
            };
            Items.Add(newWish);

            var schedule = new ScheduleSettings();
            newWish.Schedule = schedule;

            schedule.OperationContract = ServiceOperationContract.WISH_SCHEDULE;
            schedule.GenerateCommands();
            schedule.SubscribeToPropertyChanging();

            schedule.ScheduledStartDateTime = DateRange.StartDate;
            schedule.Repeat = Repeat.Daily;
            schedule.Every = 1;
        }

        protected override void TemplateChange(SelectionChangedEventArgs e)
        {
            base.TemplateChange(e);

            if (NewItem == null)
            {
                return;
            }

            NewItem.Author = Template?.Author;
        }
    }
}
