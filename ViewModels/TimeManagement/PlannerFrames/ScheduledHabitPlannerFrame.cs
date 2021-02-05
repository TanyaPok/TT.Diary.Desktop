using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Commands;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.PlannerFrames
{
    public class ScheduledHabitPlannerFrame : AbstractScheduledItemPlannerFrame<Habit<ScheduleSettings>, UnscheduledHabitSummary>
    {
        public ScheduledHabitPlannerFrame(int userId, string getUnscheduledItemsOperation) : base(userId, getUnscheduledItemsOperation)
        {

        }

        public override void GenerateCommands()
        {
            InitializeItemCreateCommand = new InitializeCreateCommand(async () => { await InitializeItem(); }, CanInitializeItem, string.Empty, true);
            ItemDeleteCommand = new DeleteCommand<Habit<ScheduleSettings>>(async (habit) => { await DeleteItem(habit); }, CanDeleteItem, string.Empty, true);
            RescheduleItemCommand = new DeleteCommand<Habit<ScheduleSettings>>(async (habit) => { await RescheduleItem(habit); }, CanRescheduleItem, "Schedule", true);
            TemplateChangeCommand = new RelayCommand<SelectionChangedEventArgs>(TemplateChange, canExecute: e => true);
            ItemSaveCommand = new SaveCommand(async () => { await SaveItem(); }, CanSaveItem, true);
        }

        protected override void FillUnscheduledItemSummaries(IEnumerable<Habit<ScheduleSettings>> data)
        {
            foreach (var item in data)
            {
                UnscheduledItemSummaries.Add(
                    new UnscheduledHabitSummary()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Amount = item.Amount,
                        ParentId = item.ParentId
                    });
            }
        }

        protected override bool CanInitializeItem()
        {
            return NewItem == null;
        }

        protected override async Task InitializeItem()
        {
            await SetUnscheduledData();

            var newHabit = new Habit<ScheduleSettings>();
            Items.Add(newHabit);

            var schedule = new ScheduleSettings();
            newHabit.Schedule = schedule;

            schedule.OperationContract = ServiceOperationContract.HABIT_SCHEDULE;
            schedule.GenerateCommands();
            schedule.SubscribeToPropertyChanging();

            schedule.ScheduledStartDateTime = PlannerDate;
            schedule.Repeat = Repeat.Daily;
            schedule.Every = 1;

            var tracker = new Tracker() { ScheduledDate = PlannerDate };
            tracker.OperationContract = ServiceOperationContract.HABIT_TRACKER;
            schedule.Trackers.Add(tracker);
        }

        protected override bool CanDeleteItem(Habit<ScheduleSettings> habit)
        {
            return habit != null && habit.CanRemove();
        }

        protected override async Task DeleteItem(Habit<ScheduleSettings> habit)
        {
            await habit.Remove();
            Items.Remove(habit);
            NewItemDescription = null;
        }

        protected override bool CanRescheduleItem(Habit<ScheduleSettings> habit)
        {
            return habit != null && habit.Schedule != null && habit.Schedule.CanRemove();
        }

        protected override async Task RescheduleItem(Habit<ScheduleSettings> habit)
        {
            await habit.Reschedule();
            Items.Remove(habit);
        }

        protected override void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var habit = (Habit<ScheduleSettings>)item;

                        if (habit.Schedule != null)
                        {
                            habit.Schedule.ParentId = habit.Id;

                            foreach (var tracker in habit.Schedule.Trackers)
                            {
                                tracker.ParentId = habit.Id;
                                tracker.OperationContract = ServiceOperationContract.HABIT_TRACKER;
                            }

                            habit.Schedule.OperationContract = ServiceOperationContract.HABIT_SCHEDULE;
                            habit.Schedule.GenerateCommands();
                            habit.Schedule.SubscribeToPropertyChanging();
                        }

                        habit.GenerateCommands();
                        habit.SubscribeToPropertyChanging();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var element = (Habit<ScheduleSettings>)item;
                        element.Dispose();
                    }
                    break;
                default: throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(e.Action));
            }
        }

        protected override bool CanSaveItem()
        {
            return NewItem != null && NewItem.IsChanged && !NewItem.HasErrors;
        }

        protected override async Task SaveItem()
        {
            var newItem = NewItem;

            if (Template == null)
            {
                newItem.ParentId = RootCategoryId;
            }
            else
            {
                newItem.ParentId = Template.ParentId;
                newItem.Id = Template.Id;
            }

            await newItem.AcceptChanges();
            NewItemDescription = null;
        }
    }
}
