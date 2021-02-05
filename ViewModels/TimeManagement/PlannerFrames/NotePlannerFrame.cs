using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class NotePlannerFrame : AbstractItemPlannerFrame<Note>
    {
        public NotePlannerFrame(int userId) : base(userId)
        {

        }

        public override void GenerateCommands()
        {
            ItemSaveCommand = new SaveCommand(async () => { await SaveItem(); }, CanSaveItem, true);
            ItemDeleteCommand = new DeleteCommand<Note>(async (item) => { await DeleteItem(item); }, CanDeleteItem, string.Empty, true);
        }

        protected override bool CanDeleteItem(Note note)
        {
            return note != null && note.CanRemove();
        }

        protected override async Task DeleteItem(Note note)
        {
            await note.Remove();
            Items.Remove(note);
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
                            element.ScheduleDate = PlannerDate;
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
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        protected override bool CanSaveItem()
        {
            return NewItem != null && NewItem.CanAcceptChanges();
        }

        protected override async Task SaveItem()
        {
            await NewItem.AcceptChanges();
        }
    }
}
