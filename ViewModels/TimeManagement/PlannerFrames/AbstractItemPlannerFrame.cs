using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.RemoveCommands;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractItemPlannerFrame<T> : ObservableObjectWithNotifyDataErrorInfo, IRemover<T>
        where T : AbstractListItem, IRemovable, IDisposable
    {
        protected int UserId { get; }

        internal T NewItem
        {
            get { return Items.FirstOrDefault(i => i.Id == default); }
        }

        public string NewItemDescription
        {
            get => NewItem?.Description;
            set
            {
                if (NewItem == null)
                {
                    return;
                }

                NewItem.Description = value;
            }
        }

        public MtObservableCollection<T> Items { get; }
        public IAttributedCommand ItemDeleteCommand { get; private set; }
        public DateRange DateRange { get; }

        #region implementation IRemover<T>

        public bool CanRemove(T item)
        {
            return true;
        }

        public virtual void Remove(T item)
        {
            item.Notify(new DirtyData() {Source = item, Operation = OperationType.Remove});
            Items.Remove(item);
        }

        #endregion

        internal void RemoveNewNotSavedEntities()
        {
            if (ItemDeleteCommand.CanExecute(NewItem))
            {
                ItemDeleteCommand.Execute(NewItem);
            }
        }

        internal void SetDateRange(DateTime startDateTime, DateTime finishDateTime)
        {
            DateRange.StartDate = startDateTime;
            DateRange.FinishDate = finishDateTime;
        }

        internal void ReUploadItems(IEnumerable<T> newItems)
        {
            Items.ReUpload(newItems);
        }

        internal virtual void GenerateCommands()
        {
            ItemDeleteCommand = new RemoveCommand<T, AbstractItemPlannerFrame<T>>(this, string.Empty, true);
        }

        protected AbstractItemPlannerFrame(int userId)
        {
            UserId = userId == default
                ? throw new ArgumentOutOfRangeException(nameof(userId))
                : userId;
            DateRange = new DateRange();
            Items = new MtObservableCollection<T>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        protected abstract void Prepare(T element);

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var element = (T) item;
                        Prepare(element);
                        element.GenerateCommands();
                        element.SubscribeToPropertyChanging();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var element = (T) item;
                        element.Dispose();
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }
    }
}