using System;
using System.Collections.Specialized;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class NotePlannerFrame : AbstractItemPlannerFrame<Note>
    {
        public NotePlannerFrame(int userId) : base(userId)
        {
        }

        protected override void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var element = (Note)item;

                        if (element.ScheduleDate == DateTime.MinValue)
                        {
                            element.ScheduleDate = DateRange.StartDate;
                        }

                        element.UserId = UserId;
                        element.GenerateCommands();
                        element.SubscribeToPropertyChanging();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var element = (Note)item;
                        element.Dispose();
                    }
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }
    }
}
