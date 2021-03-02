using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.DataContexts;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;
using TT.Diary.Desktop.Views.Controls.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractScheduledItemPlannerFrame<T, P> : AbstractItemPlannerFrame<T>, IScheduledListSource
        where T : AbstractScheduledItem<ScheduleSettings>, new()
        where P : UnscheduledItemSummary, new()
    {
        private readonly string _getUnscheduledItemsOperation;
        private readonly string _getTrackerOperation;
        private readonly string _getScheduleOperation;

        protected int RootCategoryId { get; set; }

        protected P Template { get; set; }

        public MTObservableCollection<P> UnscheduledItemSummaries { get; protected set; }

        public IAttributedCommand InitializeItemCreateCommand { get; protected set; }

        public IAttributedCommand RescheduleItemCommand { get; protected set; }

        public IAttributedCommand CompleteItemCommand { get; protected set; }

        public ICommand TemplateChangeCommand { get; protected set; }

        public AbstractScheduledItemPlannerFrame(int userId, string getUnscheduledItemsOperation, string getScheduleOperation, string getTrackerOperation) : base(userId)
        {
            _getUnscheduledItemsOperation =
                string.IsNullOrEmpty(getUnscheduledItemsOperation) || string.IsNullOrWhiteSpace(getUnscheduledItemsOperation) ?
                throw new ArgumentNullException(nameof(getUnscheduledItemsOperation)) :
                getUnscheduledItemsOperation;
            _getScheduleOperation = string.IsNullOrEmpty(getScheduleOperation) || string.IsNullOrWhiteSpace(getScheduleOperation) ?
                throw new ArgumentNullException(nameof(getScheduleOperation)) :
                getScheduleOperation;
            _getTrackerOperation = string.IsNullOrEmpty(getTrackerOperation) || string.IsNullOrWhiteSpace(getTrackerOperation) ?
                throw new ArgumentNullException(nameof(getTrackerOperation)) :
                getTrackerOperation;
            UnscheduledItemSummaries = new MTObservableCollection<P>();
        }

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            InitializeItemCreateCommand = new InitializeCreateCommand(async () => { await InitializeItem(); }, CanInitializeItem, string.Empty, true);
            RescheduleItemCommand = new DeleteCommand<T>(async (item) => { await RescheduleItem(item); }, CanRescheduleItem, "Schedule", true);
            CompleteItemCommand = new CompleteCommand<T>(async (item) => { await CompleteItem(item); }, CanCompleteItem, true);
            TemplateChangeCommand = new RelayCommand<SelectionChangedEventArgs>(TemplateChange, canExecute: e => true);
        }

        internal async Task SetUnscheduledData()
        {
            var rootCategory = await GetUnscheduledItemsCategory();
            RootCategoryId = rootCategory.Id == INITIALIZATION_IDENTIFIER
                ? throw new ArgumentOutOfRangeException(nameof(rootCategory.Id))
                : rootCategory.Id;

            if (UnscheduledItemSummaries.Count > 0)
            {
                UnscheduledItemSummaries.Clear();
            }

            var items = rootCategory.Items.Union(rootCategory.Subcategories.GetFlatSequence(c => c.Subcategories).SelectMany(c => c.Items));
            FillUnscheduledItemSummaries(items);
        }

        protected abstract Task InitializeItem();

        protected virtual bool CanInitializeItem()
        {
            return NewItem == null;
        }

        protected override bool CanSaveItem()
        {
            return base.CanSaveItem() && !NewItem.Schedule.HasErrors && NewItem.Schedule.IsChanged;
        }

        protected virtual bool CanRescheduleItem(T item)
        {
            return item != null && item.Schedule != null && item.Schedule.CanRemove();
        }

        protected virtual async Task RescheduleItem(T item)
        {
            await item.Reschedule();
            Items.Remove(item);
        }

        protected virtual bool CanCompleteItem(T item)
        {
            return item != null && item.Schedule != null && !item.Schedule.CompletionDate.HasValue
                && item.Schedule.CheckDateRange(item.Schedule.CalculateCompletionDate());
        }

        protected virtual async Task CompleteItem(T item)
        {
            await item.Complete();
        }

        protected override void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var element = (T)item;

                        if (element.Schedule != null)
                        {
                            element.Schedule.ParentId = element.Id;

                            foreach (var tracker in element.Schedule.Trackers)
                            {
                                tracker.ParentId = element.Id;
                                tracker.OperationContract = _getTrackerOperation;
                            }

                            element.Schedule.OperationContract = _getScheduleOperation;
                            element.Schedule.GenerateCommands();
                            element.Schedule.SubscribeToPropertyChanging();
                        }

                        element.GenerateCommands();
                        element.SubscribeToPropertyChanging();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var element = (T)item;
                        element.Dispose();
                    }
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        protected virtual void FillUnscheduledItemSummaries(IEnumerable<T> data)
        {
            foreach (var item in data)
            {
                UnscheduledItemSummaries.Add(
                    new P()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        ParentId = item.ParentId
                    });
            }
        }

        protected virtual void TemplateChange(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                Template = null;
                return;
            }

            Template = (P)e.AddedItems[0];
        }

        protected virtual void PreSaveItem()
        {
            if (Template == null)
            {
                NewItem.ParentId = RootCategoryId;
            }
            else
            {
                NewItem.ParentId = Template.ParentId;
                NewItem.Id = Template.Id;
            }
        }

        private async Task<Category<T>> GetUnscheduledItemsCategory()
        {
            var requestUri = string.Format(_getUnscheduledItemsOperation, UserId);
            using (var response = await Context.DiaryHttpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Category<T>>();
                }

                throw new Exception(string.Format(ErrorMessages.GetList.GetDescription(), _getUnscheduledItemsOperation, response.StatusCode));
            }
        }
    }
}
