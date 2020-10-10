using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public sealed class ListViewModel<T> : ContentControlViewModel where T : AbstractListItem, new()
    {
        private readonly string _getOperationName;

        public ObservableCollection<Category<T>> Data { get; set; }

        public IDragAndDrop DragAndDropAlgorithm { get; private set; }

        public ICommand SelectedCategoryChangedCommand { get; private set; }

        public Category<T> SelectedCategory { get; set; }

        public ListViewModel(string getOperationName)
        {
            _getOperationName = getOperationName ?? throw new ArgumentNullException(nameof(getOperationName));
            
            DragAndDropAlgorithm = new CategoryDragAndDropAlgorithm<T>();
            
            SelectedCategoryChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(SelectedCategoryChanged, canExecute: e => true);

            Commands = new ObservableCollection<Command>
            {
                new Command(
                    string.Format(ADD_LIST_ITEM, typeof(T).Name),
                    IMG_URL_LIST_ITEM,
                    AddListItem,
                    CanAddListItem)
            };

            Data = new ObservableCollection<Category<T>>();
            Data.CollectionChanged += Data_CollectionChanged;

            Messenger.Default.Register<DiaryNotificationMessage>(
                this,
                message =>
                {
                    if (message.Notification == DAILYSCHEDULE_NOTE && typeof(T) == typeof(Note))
                    {
                        IsDataLoaded = false;
                        return;
                    }
                });
        }

        ~ListViewModel()
        {
            Data.CollectionChanged -= Data_CollectionChanged;
        }

        protected override async Task LoadData(int userId)
        {
            var requestUri = string.Format(OperationContract.REQUEST_FORMAT, _getOperationName, userId);
            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var root = await response.Content.ReadAsAsync<Category<T>>();
                    root.UserId = userId;

                    if (Data.Count > 0)
                    {
                        Data.Clear();
                    }

                    Data.Add(root);
                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetList.GetDescription(), _getOperationName, response.StatusCode));
            }
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                var category = item as Category<T>;
                category.SenderPath = nameof(ListViewModel<T>);
                category.GenerateCommands();
            }
        }

        private void SelectedCategoryChanged(RoutedPropertyChangedEventArgs<object> obj)
        {
            SelectedCategory = (Category<T>)obj.NewValue;
            foreach (var command in Commands)
            {
                command.RaiseCanExecuteChanged();
            }
        }

        private bool CanAddListItem()
        {
            return SelectedCategory != null;
        }

        private void AddListItem()
        {
            SelectedCategory.AddItem();
        }
    }
}