using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.DataContexts;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractScheduledItemPlannerFrame<T, P> : AbstractItemPlannerFrame<T>
        where T : AbstractScheduledItem<ScheduleSettings>, new()
        where P : UnscheduledItemSummary, new()
    {
        private readonly string _getUnscheduledItemsOperation;

        protected int RootCategoryId { get; set; }

        protected P Template { get; set; }

        private string _newItemDescription;
        public string NewItemDescription
        {
            get
            {
                return _newItemDescription;
            }
            set
            {
                if (value == _newItemDescription)
                {
                    return;
                }

                _newItemDescription = value;

                if (NewItem == null)
                {
                    return;
                }

                NewItem.Description = value;
            }
        }

        public MTObservableCollection<P> UnscheduledItemSummaries { get; protected set; }

        public IAttributedCommand InitializeItemCreateCommand { get; protected set; }

        public IAttributedCommand RescheduleItemCommand { get; protected set; }

        public ICommand TemplateChangeCommand { get; protected set; }

        public AbstractScheduledItemPlannerFrame(int userId, string getUnscheduledItemsOperation) : base(userId)
        {
            _getUnscheduledItemsOperation =
                string.IsNullOrEmpty(getUnscheduledItemsOperation) || string.IsNullOrWhiteSpace(getUnscheduledItemsOperation) ?
                throw new ArgumentNullException(nameof(getUnscheduledItemsOperation)) :
                getUnscheduledItemsOperation;
            UnscheduledItemSummaries = new MTObservableCollection<P>();
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

        protected abstract bool CanInitializeItem();

        protected abstract Task InitializeItem();

        protected abstract bool CanRescheduleItem(T item);

        protected abstract Task RescheduleItem(T item);

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
