using GalaSoft.MvvmLight.Messaging;
using System;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Notification;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public partial class Context
    {
        private void RegisterDirtyDataMessage()
        {
            Messenger.Default.Register<DirtyData>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    CurrentViewModel.ReceiveMessage(message);
                });
        }

        private void RegisterRefreshNoteMessage()
        {
            Messenger.Default.Register<RefreshData<Note>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case DailySchedule ds:
                            _noteListViewModel.RequestRefreshData(message.DateRange);
                            break;
                        case ListViewModel<Note> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });
        }

        private void RegisterRefreshToDoMessage()
        {
            Messenger.Default.Register<RefreshData<ToDo<ScheduleSettingsSummary>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case ListViewModel<ToDo<ScheduleSettingsSummary>> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<ToDo<ScheduleSettings>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case DailySchedule ds:
                            _toDoListViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });
        }

        private void RegisterRefreshAppointmentMessage()
        {
            Messenger.Default.Register<RefreshData<Appointment<ScheduleSettingsSummary>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case ListViewModel<Appointment<ScheduleSettingsSummary>> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            _monthlyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<Appointment<ScheduleSettings>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case DailySchedule ds:
                            _appointmentListViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            _monthlyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });
        }

        private void RegisterRefreshHabitMessage()
        {
            Messenger.Default.Register<RefreshData<Habit<ScheduleSettingsSummary>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case ListViewModel<Habit<ScheduleSettingsSummary>> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<Habit<ScheduleSettings>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case DailySchedule ds:
                            _habitListViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });
        }

        private void RegisterRefreshWishMessage()
        {
            Messenger.Default.Register<RefreshData<Wish<ScheduleSettingsSummary>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case ListViewModel<Wish<ScheduleSettingsSummary>> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<Wish<ScheduleSettings>>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == null)
                    {
                        return;
                    }

                    switch (CurrentViewModel)
                    {
                        case DailySchedule ds:
                            _wishListViewModel.RequestRefreshData(message.DateRange);
                            _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(CurrentViewModel));
                    }
                });
        }

        private void RegisterRefreshScheduleSettingsMessage()
        {
            Messenger.Default.Register<RefreshData<ScheduleSettings>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == _dailyScheduleViewModel)
                    {
                        _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                    }
                    else if (CurrentViewModel == _weeklyScheduleViewModel)
                    {
                        _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                    }

                    switch (message.OwnerType)
                    {
                        case OwnerTypes.ToDo:
                            _toDoListViewModel.RequestRefreshData(message.DateRange);
                            _yearlyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        case OwnerTypes.Appointment:
                            _appointmentListViewModel.RequestRefreshData(message.DateRange);
                            _monthlyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        case OwnerTypes.Habit:
                            _habitListViewModel.RequestRefreshData(message.DateRange);
                            break;
                        case OwnerTypes.Wish:
                            _wishListViewModel.RequestRefreshData(message.DateRange);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(message.OwnerType));
                    }
                });
        }

        private void RegisterRefreshTrackerMessage()
        {
            Messenger.Default.Register<RefreshData<Tracker>>(
                this,
                (message) =>
                {
                    if (CurrentViewModel == _dailyScheduleViewModel)
                    {
                        _weeklyScheduleViewModel.RequestRefreshData(message.DateRange);
                    }
                    else if (CurrentViewModel == _weeklyScheduleViewModel)
                    {
                        _dailyScheduleViewModel.RequestRefreshData(message.DateRange);
                    }

                    switch (message.OwnerType)
                    {
                        case OwnerTypes.ToDo:
                            _yearlyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        case OwnerTypes.Appointment:
                            _monthlyScheduleViewModel.RequestRefreshData(message.DateRange);
                            break;
                        case OwnerTypes.Habit:
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(),
                                nameof(message.OwnerType));
                    }
                });
        }
    }
}