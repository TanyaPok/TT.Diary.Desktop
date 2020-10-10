using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Category<T> : ObservableObjectWithNotifyDataErrorInfo, IMessaging where T : AbstractListItem, new()
    {
        private int _oldParentId;

        public int Id { get; set; }

        public int ParentId { get; set; }

        private int _userId;
        public int UserId
        {
            get
            {
                return _userId;
            }
            internal set
            {
                _userId = value;

                foreach (var child in Subcategories)
                {
                    child.UserId = value;
                }
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
        public ObservableCollection<T> Items { get; set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand SaveEditedCategoryCommand { get; private set; }
        public RelayCommand<Category<T>> RemoveCommand { get; private set; }
        public RelayCommand<T> RemoveItemCommand { get; private set; }

        private string _senderPath;
        public string SenderPath
        {
            get
            {
                return _senderPath;
            }
            set
            {
                _senderPath = value;

                foreach (var item in Items)
                {
                    item.SenderPath = string.Format(MESSAGING_FORMAT, value, typeof(T).Name);
                }
            }
        }

        public Category()
        {
            Description = "New Category";
            Subcategories = new ObservableCollection<Category<T>>();
            Subcategories.CollectionChanged += Subcategories_CollectionChanged;
            Items = new ObservableCollection<T>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        ~Category()
        {
            Subcategories.CollectionChanged -= Subcategories_CollectionChanged;
            Items.CollectionChanged -= Items_CollectionChanged;
        }

        public bool HasSubCategory(Category<T> category)
        {
            return Subcategories.GetFlatSequence(c => c.Subcategories).Any(c => c == category);
        }

        internal void GenerateCommands(Category<T> parent = null)
        {
            AddCommand = new RelayCommand(() =>
            {
                Subcategories.Add(new Category<T> { IsReadOnlyMode = false });
            });

            EditCommand = new RelayCommand(() => { IsReadOnlyMode = false; }, () => { return ParentId > 0; });

            SaveEditedCategoryCommand = new RelayCommand(() =>
            {
                if (Id == 0)
                {
                    Add();
                }
                else
                {
                    Edit();
                }

                IsReadOnlyMode = true;
            },
            () =>
            {
                return !HasErrors;
            });

            if (parent == null)
            {
                RemoveCommand = new RelayCommand<Category<T>>((item) => { }, item => false);
            }
            else
            {
                RemoveCommand = new RelayCommand<Category<T>>(parent.Remove, CanRemove);
            }

            RemoveItemCommand = new RelayCommand<T>(RemoveItem, CanRemoveItem);
        }

        internal void AddItem()
        {
            Items.Add(new T());
        }

        internal async void Edit(Category<T> parent = null, Category<T> oldParent = null)
        {
            var category = new { Id, Description, OldCategoryId = oldParent?.Id ?? _oldParentId, CategoryId = parent?.Id ?? ParentId };
            using (var response = await Context.DiaryHttpClient.PutAsJsonAsync(OperationContract.CATEGORY, category))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception(string.Format(ErrorMessages.Edit.GetDescription(), "Category " + Description, errorMessage));
                }

                if (oldParent != null)
                {
                    oldParent.Subcategories.Remove(this);
                }

                if (parent != null)
                {
                    parent.Subcategories.Add(this);
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
                        var category = item as Category<T>;
                        category.SenderPath = SenderPath;
                        category.UserId = UserId;
                        category.GenerateCommands(this);
                        category.ParentId = Id;
                    }

                    RemoveCommand?.RaiseCanExecuteChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var category = item as Category<T>;
                        category._oldParentId = Id;
                    }

                    RemoveCommand?.RaiseCanExecuteChanged();
                    break;
                default: throw new NotImplementedException();
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var element = (AbstractListItem)item;
                        element.ParentId = Id;
                        element.SenderPath = string.Format(MESSAGING_FORMAT, SenderPath, typeof(T).Name);
                        element.SaveCommand = new RelayCommand(element.SaveAsync, element.CanSave);
                    }

                    RemoveCommand?.RaiseCanExecuteChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveCommand?.RaiseCanExecuteChanged();
                    break;
                default: throw new NotImplementedException();
            }
        }

        private async void Add()
        {
            var category = new { Description, CategoryId = ParentId, UserId };
            using (var response = await Context.DiaryHttpClient.PostAsJsonAsync(OperationContract.CATEGORY, category))
            {
                if (response.IsSuccessStatusCode)
                {
                    Id = await response.Content.ReadAsAsync<int>();
                    return;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(string.Format(ErrorMessages.Add.GetDescription(), "Category " + Description, errorMessage));
            }
        }

        private bool CanRemove(Category<T> category)
        {
            return !(category.Items?.Count > 0 || category.Subcategories?.Count > 0);
        }

        private async void Remove(Category<T> category)
        {
            var requestUri = string.Format(OperationContract.REQUEST_FORMAT, OperationContract.CATEGORY, category.Id);
            using (var response = await Context.DiaryHttpClient.DeleteAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    Subcategories.Remove(category);
                    return;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(string.Format(ErrorMessages.Remove.GetDescription(), "Category " + category.Description, errorMessage));
            }
        }

        private bool CanRemoveItem(T item)
        {
            return item == null || item.CanRemove();
        }

        private async void RemoveItem(T item)
        {
            if (await item.RemoveAsync())
            {
                Items.Remove((T)item);
            }
        }
    }
}