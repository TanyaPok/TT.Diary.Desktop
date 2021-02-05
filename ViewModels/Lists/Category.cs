using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Category<T> : AbstractEntity, IEntityContainerCommands, IPublisher<DirtyData> where T : AbstractListItem, new()
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return ServiceOperationContract.CATEGORY;
            }
        }

        private string _description;
        public string Description
        {
            set
            {
                ClearErrors(nameof(Description));

                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    AddError(nameof(Description), ValidationMessages.EmptyDescription.GetDescription());
                }

                Set(ref _description, value);
            }
            get
            {
                return _description;
            }
        }

        private bool _isReadOnlyMode = true;
        public bool IsReadOnlyMode
        {
            set
            {
                Set(ref _isReadOnlyMode, value);
            }
            get
            {
                return _isReadOnlyMode;
            }
        }

        public ObservableCollection<Category<T>> Subcategories { get; private set; }

        public ObservableCollection<T> Items { get; private set; }

        #region ICUDCommands
        public IAttributedCommand InitializeNestedEntityCreateCommand { get; private set; }

        public IAttributedCommand InitializeNestedEntityUpdateCommand { get; private set; }

        public IAttributedCommand NestedEntityDeleteCommand { get; private set; }
        #endregion

        public IAttributedCommand InitializeAddCategoryItemCommand { get; private set; }

        public IAttributedCommand RemoveItemCommand { get; private set; }

        public Category()
        {
            Description = string.Empty;
            Subcategories = new ObservableCollection<Category<T>>();
            Subcategories.CollectionChanged += Subcategories_CollectionChanged;
            Items = new ObservableCollection<T>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            Subcategories.CollectionChanged -= Subcategories_CollectionChanged;
            Items.CollectionChanged -= Items_CollectionChanged;
        }

        public bool HasSubCategory(Category<T> category)
        {
            return Subcategories.GetFlatSequence(c => c.Subcategories).Any(c => c == category);
        }

        public void Notify(DirtyData message)
        {
            using (var manager = new DirtyDataManager())
            {
                manager.Send(message);
            }
        }

        internal void GenerateCategoryCommands(Category<T> parent = null)
        {
            GenerateCommands();

            InitializeAddCategoryItemCommand = new InitializeCreateCommand(
                () =>
                {
                    var newItem = new T();
                    Items.Add(newItem);
                },
                () => { return true; },
                typeof(T).GetNameWithoutGenericArity());

            InitializeNestedEntityCreateCommand = new InitializeCreateCommand(
                () =>
                {
                    Subcategories.Add(new Category<T> { IsReadOnlyMode = false });
                },
                () => { return true; },
                typeof(Category<T>).GetNameWithoutGenericArity());

            InitializeNestedEntityUpdateCommand = new InitializeUpdateCommand(
                () =>
                {
                    IsReadOnlyMode = false;
                },
                () => { return ParentId > INITIALIZATION_IDENTIFIER; },
                typeof(Category<T>).GetNameWithoutGenericArity());

            if (parent == null)
            {
                NestedEntityDeleteCommand = new DeleteCommand<Category<T>>((item) => { throw new NotImplementedException(); }, item => false, typeof(Category<T>).GetNameWithoutGenericArity());
            }
            else
            {
                NestedEntityDeleteCommand = new DeleteCommand<Category<T>>(async (child) => { await parent.Remove(child); }, CanRemove, typeof(Category<T>).GetNameWithoutGenericArity(), true);
            }

            RemoveItemCommand = new DeleteCommand<T>(async (item) => { await RemoveItem(item); }, (item) => { return item != null && item.CanRemove(); }, string.Empty, true);
        }

        internal async Task Move(Category<T> parent, Category<T> oldParent)
        {
            await Endpoint.UpdateEntity(
                 new Request
                 {
                     OperationContract = ServiceOperationContract.CATEGORY,
                     Data = new { Id, Description, OldCategoryId = oldParent.Id, CategoryId = parent.Id },
                     AdditionalInfo = "Category " + Description
                 });
            oldParent.Subcategories.Remove(this);
            parent.Subcategories.Add(this);
        }

        protected async override Task Save()
        {
            await base.Save();
            IsReadOnlyMode = true;
            Notify(new DirtyData { Source = this, Operation = OperationType.Remove });
        }

        protected override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.CATEGORY,
                Data = new { Description, CategoryId = ParentId, UserId },
                AdditionalInfo = "Category " + Description
            };
        }

        protected override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.CATEGORY,
                Data = new { Id, Description, CategoryId = ParentId },
                AdditionalInfo = "Category " + Description
            };
        }

        protected override void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.EntityPropertyChanged(sender, e);
            Notify(new DirtyData { Source = this, Operation = OperationType.Add });
        }

        private void Subcategories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var category = (Category<T>)item;
                        category.UserId = UserId;
                        category.ParentId = Id;
                        category.GenerateCategoryCommands(this);
                        category.SubscribeToPropertyChanging();
                    }

                    NestedEntityDeleteCommand?.RaiseCanExecuteChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        ((Category<T>)item).Dispose();
                    }

                    NestedEntityDeleteCommand?.RaiseCanExecuteChanged();
                    break;
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var element = (T)item;
                        element.ParentId = Id;
                        element.UserId = UserId;
                        element.GenerateCommands();
                        element.SubscribeToPropertyChanging();
                    }

                    NestedEntityDeleteCommand?.RaiseCanExecuteChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        ((T)item).Dispose();
                    }

                    NestedEntityDeleteCommand?.RaiseCanExecuteChanged();
                    break;
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        private bool CanRemove(Category<T> category)
        {
            return !(category.Items?.Count > 0 || category.Subcategories?.Count > 0);
        }

        private async Task Remove(Category<T> category)
        {
            await category.Remove();
            Subcategories.Remove(category);
        }

        private async Task RemoveItem(T item)
        {
            await item.Remove();
            Items.Remove(item);
        }
    }
}