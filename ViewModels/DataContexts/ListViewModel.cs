using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
    public sealed class ListViewModel<T> : AbstractContentControlViewModel where T : AbstractListItem, new()
    {
        private readonly int _userId;

        private readonly string _listGettingOperationName;

        public ObservableCollection<Category<T>> Data { get; set; }

        public IDragAndDrop DragAndDropAlgorithm { get; private set; }

        public ICommand SelectedCategoryChangedCommand { get; private set; }

        public ListViewModel(string listGettingOperationName, int userId)
        {
            _userId = userId == INITIALIZATION_IDENTIFIER ? throw new ArgumentOutOfRangeException(nameof(userId)) : userId;
            _listGettingOperationName = listGettingOperationName ?? throw new ArgumentNullException(nameof(listGettingOperationName));

            DragAndDropAlgorithm = new CategoryDragAndDropAlgorithm<T>();

            SelectedCategoryChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(SelectedCategoryChanged, canExecute: e => true);

            Data = new ObservableCollection<Category<T>>();
            Data.CollectionChanged += Data_CollectionChanged;
        }

        ~ListViewModel()
        {
            Data.CollectionChanged -= Data_CollectionChanged;
        }

        protected override async Task LoadData()
        {
            var requestUri = string.Format(_listGettingOperationName, _userId);
            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var root = await response.Content.ReadAsAsync<Category<T>>();

                    if (Data.Count > 0)
                    {
                        Data.Clear();
                    }

                    Data.Add(root);
                    return;
                }

                throw new Exception(string.Format(ErrorMessages.GetList.GetDescription(), _listGettingOperationName, response.StatusCode));
            }
        }

        protected override Task DataSetting()
        {
            SelectedCategoryChanged(new RoutedPropertyChangedEventArgs<object>(null, Data[0]));
            return Task.CompletedTask;
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                ((Category<T>)item).GenerateCategoryCommands();
            }
        }

        private void SelectedCategoryChanged(RoutedPropertyChangedEventArgs<object> category)
        {
            var selectedCategory = (Category<T>)category.NewValue;

            if (selectedCategory == null)
            {
                return;
            }

            Commands.Clear();
            Commands.Add(selectedCategory.InitializeAddCategoryItemCommand);
        }
    }
}