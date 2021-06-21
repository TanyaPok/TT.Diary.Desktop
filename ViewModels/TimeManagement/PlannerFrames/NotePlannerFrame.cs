using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class NotePlannerFrame : AbstractItemPlannerFrame<Note>
    {
        public NotePlannerFrame(int userId) : base(userId)
        {
        }

        protected override void Prepare(Note element)
        {
            if (element.Id == default)
            {
                element.ScheduleDate = DateRange.StartDate;
            }

            element.UserId = UserId;
        }

        public override void Remove(Note item)
        {
            item.Notify();
            base.Remove(item);
        }
    }
}