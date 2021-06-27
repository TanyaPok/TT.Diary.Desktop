using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.InitializingCommands;
using TT.Diary.Desktop.ViewModels.Commands.RemoveCommands;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Notification;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Category<T> : AbstractEntity, IRemovable, IRemover<Category<T>>, IRemover<T>
        where T : AbstractListItem, IRemovable, new()
    {
        private string _description;

        [TrackChange]
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
            get => _description;
        }

        private bool _isReadOnlyMode = true;

        public bool IsReadOnlyMode
        {
            set => Set(ref _isReadOnlyMode, value);
            get => _isReadOnlyMode;
        }

        public ObservableCollection<Category<T>> Subcategories { get; }
        public ObservableCollection<T> Items { get; }

        public IAttributedCommand InitializeNestedCategoryCreateCommand { get; private set; }
        public IAttributedCommand InitializeNestedCategoryUpdateCommand { get; private set; }
        public IAttributedCommand NestedCategoryDeleteCommand { get; private set; }
        public ICommand NestedCategorySaveCommand { get; private set; }
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

        #region implementation IRemovable

        public bool CanBeRemoved()
        {
            return !(Items is {Count: > 0} || Subcategories is {Count: > 0});
        }

        public Request GetRemoveRequest()
        {
            var requestUri = string.Format(ServiceOperationContract.RequestFormat, ServiceOperationContract.Category,
                Id);
            return new Request {OperationContract = requestUri};
        }

        #endregion

        #region implementation IRemover<Category<T>>

        public bool CanRemove(Category<T> category)
        {
            return true;
        }

        public void Remove(Category<T> category)
        {
            Subcategories.Remove(category);
        }

        #endregion

        #region implementation IRemover<T>

        public void Remove(T item)
        {
            item.Notify(new DirtyData() {Source = item, Operation = OperationType.Remove});
            item.Notify();
            Items.Remove(item);
        }

        public bool CanRemove(T entity)
        {
            return true;
        }

        #endregion

        public bool HasSubCategory(Category<T> category)
        {
            return Subcategories.GetFlatSequence(c => c.Subcategories).Any(c => c == category);
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
                () => true,
                typeof(T).GetNameWithoutGenericArity());

            InitializeNestedCategoryCreateCommand = new InitializeCreateCommand(
                () => { Subcategories.Add(new Category<T> {IsReadOnlyMode = false}); },
                () => true,
                typeof(Category<T>).GetNameWithoutGenericArity());

            InitializeNestedCategoryUpdateCommand = new InitializeUpdateCommand(
                () => { IsReadOnlyMode = false; },
                () => ParentId > default(int),
                typeof(Category<T>).GetNameWithoutGenericArity());

            NestedCategorySaveCommand = new RelayCommand(async () =>
            {
                if (CanAcceptChanges())
                {
                    await AcceptChanges();
                }

                if (!HasErrors)
                {
                    IsReadOnlyMode = true;
                }
            }, !IsReadOnlyMode);

            if (parent == null)
            {
                NestedCategoryDeleteCommand =
                    new RemoveCommand<Category<T>, Category<T>>(typeof(Category<T>).GetNameWithoutGenericArity());
            }
            else
            {
                NestedCategoryDeleteCommand =
                    new RemoveCommand<Category<T>, Category<T>>(parent,
                        typeof(Category<T>).GetNameWithoutGenericArity(), true);
            }

            RemoveItemCommand = new RemoveCommand<T, Category<T>>(this, string.Empty, true);
        }

        internal async Task Move(Category<T> parent, Category<T> oldParent)
        {
            await Endpoint.UpdateAsync(
                new Request
                {
                    OperationContract = ServiceOperationContract.Category,
                    Data = new {Id, Description, OldCategoryId = oldParent.Id, CategoryId = parent.Id},
                    AdditionalInfo = "Category " + Description
                });
            oldParent.Subcategories.Remove(this);
            parent.Subcategories.Add(this);
        }

        public override Request GetCreateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Category,
                Data = new {Description, CategoryId = ParentId, UserId},
                AdditionalInfo = "Category " + Description
            };
        }

        public override Request GetUpdateRequest()
        {
            return new Request
            {
                OperationContract = ServiceOperationContract.Category,
                Data = new {Id, Description, CategoryId = ParentId},
                AdditionalInfo = "Category " + Description
            };
        }

        internal void RecursivelyRemoveNewItems()
        {
            RemoveNewItems();
            foreach (var category in Subcategories.GetFlatSequence(c => c.Subcategories).ToArray())
            {
                category.RemoveNewItems();
            }
        }

        private void RemoveNewItems()
        {
            foreach (var item in Items.ToArray())
            {
                if (item.Id == default)
                {
                    Remove(item);
                }
            }
        }

        private void Subcategories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var category = (Category<T>) item;
                        category.UserId = UserId;
                        category.ParentId = Id;
                        category.GenerateCategoryCommands(this);
                        category.SubscribeToPropertyChanging();
                    }

                    if (NestedCategoryDeleteCommand != null)
                    {
                        NestedCategoryDeleteCommand.RaiseCanExecuteChanged();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        ((Category<T>) item).Dispose();
                    }

                    if (NestedCategoryDeleteCommand != null)
                    {
                        NestedCategoryDeleteCommand.RaiseCanExecuteChanged();
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
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
                        var element = (T) item;
                        element.ParentId = Id;
                        element.UserId = UserId;
                        element.GenerateCommands();
                        element.SubscribeToPropertyChanging();
                    }

                    if (NestedCategoryDeleteCommand != null)
                    {
                        NestedCategoryDeleteCommand.RaiseCanExecuteChanged();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        ((T) item).Dispose();
                    }

                    if (NestedCategoryDeleteCommand != null)
                    {
                        NestedCategoryDeleteCommand.RaiseCanExecuteChanged();
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }
    }
}