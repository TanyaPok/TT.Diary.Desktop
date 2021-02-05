using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractItemPlannerFrame<T> where T : AbstractListItem, IDisposable
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

        internal DateTime PlannerDate { get; set; }

        public MTObservableCollection<T> Items { get; private set; }

        public IAttributedCommand ItemSaveCommand { get; protected set; }

        public IAttributedCommand ItemDeleteCommand { get; protected set; }

        public AbstractItemPlannerFrame(int userId)
        {
            UserId = userId == INITIALIZATION_IDENTIFIER ? throw new ArgumentOutOfRangeException(nameof(userId)) : userId;
            Items = new MTObservableCollection<T>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public virtual void Dispose()
        {
            Items.CollectionChanged -= Items_CollectionChanged;
        }

        public abstract void GenerateCommands();

        protected abstract void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        protected abstract bool CanDeleteItem(T item);

        protected abstract Task DeleteItem(T item);

        protected abstract bool CanSaveItem();

        protected abstract Task SaveItem();
    }
}
