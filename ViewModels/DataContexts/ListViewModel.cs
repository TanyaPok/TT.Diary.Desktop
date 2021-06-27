using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.InitializingCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public sealed class ListViewModel<T> : AbstractContentControlViewModel, ICreator where T : AbstractListItem, new()
    {
        private readonly string _listGettingOperationName;

        public ObservableCollection<Category<T>> Data { get; set; }

        public IDragAndDrop DragAndDropAlgorithm { get; }

        public ICommand SelectedCategoryChangedCommand { get; }

        public ListViewModel(string listGettingOperationName, int userId) : base(userId)
        {
            _listGettingOperationName = listGettingOperationName ??
                                        throw new ArgumentNullException(nameof(listGettingOperationName));

            IsAutosaveCapable = true;
            DragAndDropAlgorithm = new CategoryDragAndDropAlgorithm<T>();

            SelectedCategoryChangedCommand =
                new RelayCommand<RoutedPropertyChangedEventArgs<object>>(SelectedCategoryChanged,
                    canExecute: e => true);

            Data = new ObservableCollection<Category<T>>();
            Data.CollectionChanged += Data_CollectionChanged;
        }

        public void RemoveNewNotSavedEntities()
        {
            foreach (var data in Data)
            {
                data.RecursivelyRemoveNewItems();
            }
        }

        protected override bool InRangeDates(DateRange dateRange)
        {
            return true;
        }

        protected override async Task LoadDataAsync()
        {
            var requestUri = string.Format(_listGettingOperationName, UserId);
            var root = await Endpoint.GetAsync<Category<T>>(requestUri, ErrorMessages.GetList.GetDescription(),
                _listGettingOperationName);
            if (Data.Count > 0)
            {
                Data.Clear();
            }

            Data.Add(root);
        }

        protected override Task DataSettingAsync()
        {
            SelectedCategoryChanged(new RoutedPropertyChangedEventArgs<object>(null, Data[0]));
            return Task.CompletedTask;
        }

        private static void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                ((Category<T>) item).GenerateCategoryCommands();
            }
        }

        private void SelectedCategoryChanged(RoutedPropertyChangedEventArgs<object> category)
        {
            var selectedCategory = (Category<T>) category.NewValue;

            if (selectedCategory == null)
            {
                return;
            }

            Commands.Clear();
            Commands.Add(selectedCategory.InitializeAddCategoryItemCommand);
        }
    }
}