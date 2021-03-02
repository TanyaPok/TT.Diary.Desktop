using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.Views.Controls.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractItemPlannerFrame<T> : ObservableObjectWithNotifyDataErrorInfo where T : AbstractListItem, IDisposable
    {
        protected readonly int INITIALIZATION_IDENTIFIER = 0;

        protected int UserId { get; set; }

        protected T NewItem
        {
            get
            {
                return Items.FirstOrDefault(i => i.Id == INITIALIZATION_IDENTIFIER);
            }
        }

        public string NewItemDescription
        {
            get
            {
                return NewItem?.Description;
            }
            set
            {
                if (NewItem == null)
                {
                    return;
                }

                NewItem.Description = value;
            }
        }

        public MTObservableCollection<T> Items { get; private set; }

        public IAttributedCommand ItemSaveCommand { get; protected set; }

        public IAttributedCommand ItemDeleteCommand { get; protected set; }

        public DateRange DateRange { get; private set; }

        public AbstractItemPlannerFrame(int userId)
        {
            UserId = userId == INITIALIZATION_IDENTIFIER ? throw new ArgumentOutOfRangeException(nameof(userId)) : userId;
            DateRange = new DateRange();
            Items = new MTObservableCollection<T>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public virtual void Dispose()
        {
            Items.CollectionChanged -= Items_CollectionChanged;
        }

        internal virtual void GenerateCommands()
        {
            ItemSaveCommand = new SaveCommand(async () => { await SaveItem(); }, CanSaveItem, true);
            ItemDeleteCommand = new DeleteCommand<T>(async (item) => { await DeleteItem(item); }, CanDeleteItem, string.Empty, true);
        }

        protected abstract void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        protected virtual bool CanDeleteItem(T item)
        {
            return item != null && item.CanRemove();
        }

        protected virtual async Task DeleteItem(T item)
        {
            await item.Remove();
            Items.Remove(item);
        }

        protected virtual bool CanSaveItem()
        {
            return NewItem != null && NewItem.CanAcceptChanges();
        }

        protected virtual async Task SaveItem()
        {
            await NewItem.AcceptChanges();
        }
    }
}
