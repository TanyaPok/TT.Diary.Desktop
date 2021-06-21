using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Commands.InitializingCommands;
using TT.Diary.Desktop.ViewModels.Commands.RemoveCommands;
using TT.Diary.Desktop.ViewModels.Commands.SaveCommands;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public abstract class AbstractScheduledItemPlannerFrame<T, TP> : AbstractItemPlannerFrame<T>,
        IScheduledListSource,
        IStoreKeeper<AbstractScheduledItem<ScheduleSettings>>, IStoreKeeper<ScheduleSettings>,
        IRemover<ScheduleSettings>
        where T : AbstractScheduledItem<ScheduleSettings>, new()
        where TP : UnscheduledItemSummary, new()
    {
        private readonly string _getUnscheduledItemsOperation;
        private readonly string _getScheduleOperation;
        private int RootCategoryId { get; set; }
        protected TP Template { get; private set; }
        public MtObservableCollection<TP> UnscheduledItemSummaries { get; }
        public IAttributedCommand InitializeItemCreateCommand { get; private set; }
        public IAttributedCommand ItemSaveCommand { get; private set; }
        public IAttributedCommand ItemRescheduleCommand { get; private set; }
        public IAttributedCommand ItemCompleteCommand { get; private set; }
        public ICommand TemplateChangeCommand { get; protected set; }

        #region implementation IStoreKeeper<AbstractScheduledItem<ScheduleSettings>>

        public virtual bool CanAcceptChanges(AbstractScheduledItem<ScheduleSettings> storable)
        {
            return storable != null && storable.CanAcceptChanges() && storable.Schedule.CanAcceptChanges();
        }

        public virtual void BeforeAcceptChanges(AbstractScheduledItem<ScheduleSettings> storable)
        {
            if (Template == null)
            {
                storable.ParentId = RootCategoryId;
            }
            else
            {
                storable.ParentId = Template.ParentId;
                storable.Id = Template.Id;
            }
        }

        public virtual async Task AfterAcceptChanges(AbstractScheduledItem<ScheduleSettings> storable)
        {
            storable.Schedule.ParentId = storable.Id;
            await storable.Schedule.AcceptChanges();
        }

        #endregion

        #region implementation IStoreKeeper<ScheduleSettings>

        public bool CanAcceptChanges(ScheduleSettings storable)
        {
            return storable != null && !storable.CompletionDate.HasValue &&
                   storable.CheckDateRange(storable.CalculateCompletionDate());
        }

        public void BeforeAcceptChanges(ScheduleSettings storable)
        {
            storable.CompletionDate = storable.CalculateCompletionDate();
        }

        public Task AfterAcceptChanges(ScheduleSettings storable)
        {
            storable.Notify();
            return Task.CompletedTask;
        }

        #endregion

        public override void Remove(T item)
        {
            item.Schedule.Notify(new DirtyData() {Source = item.Schedule, Operation = OperationType.Remove});
            base.Remove(item);
        }

        #region implementation IRemover<ScheduleSettings>

        public bool CanRemove(ScheduleSettings entity)
        {
            return true;
        }

        public void Remove(ScheduleSettings entity)
        {
            var item = Items.First(i => i.Schedule == entity);
            Items.Remove(item);
            entity.Notify();
        }

        #endregion

        internal override void GenerateCommands()
        {
            base.GenerateCommands();
            InitializeItemCreateCommand = new InitializeCreateCommand(async () => { await InitializeItem(); },
                CanInitializeItem, string.Empty, true);
            ItemSaveCommand = new SaveCommand<T, AbstractScheduledItemPlannerFrame<T, TP>>(this, true);
            ItemRescheduleCommand =
                new RemoveCommand<ScheduleSettings, AbstractScheduledItemPlannerFrame<T, TP>>(this, "Schedule", true);
            ItemCompleteCommand =
                new SaveCommand<ScheduleSettings, AbstractScheduledItemPlannerFrame<T, TP>>(this, true, "Complete",
                    "pack://application:,,,/Images/Toolbar/done.png");
            TemplateChangeCommand = new RelayCommand<SelectionChangedEventArgs>(TemplateChange, true);
        }

        internal async Task SetUnscheduledData()
        {
            var rootCategory = await GetUnscheduledItemsCategory();
            RootCategoryId = rootCategory.Id == default
                ? throw new ArgumentOutOfRangeException(nameof(rootCategory.Id))
                : rootCategory.Id;

            if (UnscheduledItemSummaries.Count > 0)
            {
                UnscheduledItemSummaries.Clear();
            }

            var items = rootCategory.Items.Union(rootCategory.Subcategories.GetFlatSequence(c => c.Subcategories)
                .SelectMany(c => c.Items));
            FillUnscheduledItemSummaries(items);
        }

        protected AbstractScheduledItemPlannerFrame(int userId, string getUnscheduledItemsOperation,
            string getScheduleOperation) :
            base(userId)
        {
            _getUnscheduledItemsOperation =
                string.IsNullOrEmpty(getUnscheduledItemsOperation) ||
                string.IsNullOrWhiteSpace(getUnscheduledItemsOperation)
                    ? throw new ArgumentNullException(nameof(getUnscheduledItemsOperation))
                    : getUnscheduledItemsOperation;
            _getScheduleOperation =
                string.IsNullOrEmpty(getScheduleOperation) || string.IsNullOrWhiteSpace(getScheduleOperation)
                    ? throw new ArgumentNullException(nameof(getScheduleOperation))
                    : getScheduleOperation;
            UnscheduledItemSummaries = new MtObservableCollection<TP>();
        }

        protected override void Prepare(T element)
        {
            element.Schedule.ParentId = element.Id;
            element.Schedule.OperationContract = _getScheduleOperation;
            element.Schedule.OwnerType =
                (OwnerTypes) Enum.Parse(typeof(OwnerTypes), typeof(T).GetNameWithoutGenericArity());
            element.Schedule.GenerateCommands();
            element.Schedule.SubscribeToPropertyChanging();
        }

        protected abstract Task InitializeItem();

        protected virtual void FillUnscheduledItemSummaries(IEnumerable<T> data)
        {
            foreach (var item in data)
            {
                UnscheduledItemSummaries.Add(
                    new TP()
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

            Template = (TP) e.AddedItems[0];
        }

        private bool CanInitializeItem()
        {
            return NewItem == null;
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

                throw new Exception(string.Format(ErrorMessages.GetList.GetDescription(), _getUnscheduledItemsOperation,
                    response.StatusCode));
            }
        }
    }
}