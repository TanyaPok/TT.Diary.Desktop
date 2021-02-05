using System.Collections.Generic;
using System.Configuration;
using System.Windows.Input;
using TT.Diary.Desktop.Configs;
using System;
using GalaSoft.MvvmLight.CommandWpf;
using System.Net.Http;
using System.Net.Http.Headers;
using TT.Diary.Desktop.ViewModels.Common;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Linq;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.TimeManagement;
using TT.Diary.Desktop.ViewModels.Notification;
using GalaSoft.MvvmLight.Messaging;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class Context : ViewModelBase
    {
        private readonly string SCHEDULE_TYPES = "scheduleTypes";
        private readonly string LIST_TYPES = "listTypes";
        private readonly string THEMES = "themes";

        private readonly DailySchedule _dailyScheduleViewModel;
        private readonly WeeklySchedule _weeklyScheduleViewModel;
        private readonly MonthlySchedule _monthlyScheduleViewModel;
        private readonly YearlySchedule _yearlyScheduleViewModel;
        private readonly ListViewModel<Wish<ScheduleSettingsSummary>> _wishListViewModel;
        private readonly ListViewModel<ToDo<ScheduleSettingsSummary>> _toDoListViewModel;
        private readonly ListViewModel<Habit<ScheduleSettingsSummary>> _habitListViewModel;
        private readonly ListViewModel<Note> _noteListViewModel;

        private readonly User _user;

        internal static readonly HttpClient DiaryHttpClient;

        public const string PRODUCTIVITY = "productivities";
        public static readonly IDictionary<ProductivityGradation, Productivity> Productivities;

        public string DictionaryTip { get { return "Show diary to-do lists"; } }
        public IList<Theme> Themes { get; private set; }
        public IList<ScheduleType> ScheduleTypesMenu { get; private set; }
        public IList<ListType> ListTypesMenu { get; private set; }
        public IDictionary<string, ICommand> ScheduleCommands { get; private set; }
        public IDictionary<string, ICommand> ListCommands { get; private set; }
        public ICommand ChangeThemeCommand { get; private set; }

        public string WindowTitle { get { return string.Format("{0}'s diary", _user.Given_Name); } }

        private AbstractContentControlViewModel _currentViewModel;
        public AbstractContentControlViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                Set(ref _currentViewModel, value);
            }
        }

        private Theme _theme;
        public Theme Theme
        {
            set
            {
                Set(ref _theme, value);
            }
            get
            {
                return _theme;
            }
        }

        static Context()
        {
            var url = ConfigurationManager.ConnectionStrings["WebApiUrl"].ConnectionString;
            DiaryHttpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            DiaryHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var productivities = ConfigurationManager.GetSection(PRODUCTIVITY) as IList<Productivity>;
            Productivities = new Dictionary<ProductivityGradation, Productivity>();
            foreach (var productivity in productivities)
            {
                Productivities.Add(productivity.Gradation, productivity);
            }
        }

        public Context(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));

            Themes = ConfigurationManager.GetSection(THEMES) as IList<Theme>;
            ScheduleTypesMenu = ConfigurationManager.GetSection(SCHEDULE_TYPES) as IList<ScheduleType>;
            ListTypesMenu = ConfigurationManager.GetSection(LIST_TYPES) as IList<ListType>;

            _dailyScheduleViewModel = new DailySchedule(_user.Id);
            _weeklyScheduleViewModel = new WeeklySchedule();
            _monthlyScheduleViewModel = new MonthlySchedule(_user.Id);
            _yearlyScheduleViewModel = new YearlySchedule(_user.Id);

            _wishListViewModel = new ListViewModel<Wish<ScheduleSettingsSummary>>(ServiceOperationContract.GET_WISH_LIST, _user.Id);
            _toDoListViewModel = new ListViewModel<ToDo<ScheduleSettingsSummary>>(ServiceOperationContract.GET_TODO_LIST, _user.Id);
            _habitListViewModel = new ListViewModel<Habit<ScheduleSettingsSummary>>(ServiceOperationContract.GET_HABITS, _user.Id);
            _noteListViewModel = new ListViewModel<Note>(ServiceOperationContract.GET_NOTES, _user.Id);

            ChangeThemeCommand = new RelayCommand(ChangeTheme, canExecute: () => true);
            SetScheduleCommands();
            SetListCommands();

            SetDefaultTheme();
            SetDefaultSchedule();
            Subscribe();
        }

        ~Context()
        {
            Cleanup();
        }

        private void SetDefaultSchedule()
        {
            if (ScheduleCommands[ScheduleType.DAY].CanExecute(null))
            {
                ScheduleCommands[ScheduleType.DAY].Execute(null);
            }
        }

        private void SetDefaultTheme()
        {
            Theme = Themes.First();
        }

        private void ChangeTheme()
        {
            if (Theme == null)
            {
                SetDefaultTheme();
            }
            else
            {
                switch (Theme.Name)
                {
                    case Theme.TEA:
                        Theme = Themes.Single(t => t.Name == Theme.COFFEE);
                        break;
                    case Theme.COFFEE:
                        Theme = Themes.Single(t => t.Name == Theme.TEA);
                        break;
                    default:
                        throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(), Theme.Name));
                }
            }

            ((IResourceService)App.Current).ChangeTheme(Theme.Source);
        }

        private void SetListCommands()
        {
            ListCommands = new Dictionary<string, ICommand>();
            foreach (var listType in ListTypesMenu)
            {
                switch (listType.Name)
                {
                    case ListType.WISH_LIST:
                        ListCommands.Add(listType.Name, new RelayCommand(async () => { await UpdateContent(_wishListViewModel); }, true));
                        break;
                    case ListType.TODO_LIST:
                        ListCommands.Add(listType.Name, new RelayCommand(async () => { await UpdateContent(_toDoListViewModel); }, true));
                        break;
                    case ListType.HABITS:
                        ListCommands.Add(listType.Name, new RelayCommand(async () => { await UpdateContent(_habitListViewModel); }, true));
                        break;
                    case ListType.NOTES:
                        ListCommands.Add(listType.Name, new RelayCommand(async () => { await UpdateContent(_noteListViewModel); }, true));
                        break;
                    default:
                        throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(), listType.Name));
                }
            }
        }

        private void SetScheduleCommands()
        {
            ScheduleCommands = new Dictionary<string, ICommand>();
            foreach (var scheduleType in ScheduleTypesMenu)
            {
                switch (scheduleType.Name)
                {
                    case ScheduleType.DAY:
                        ScheduleCommands.Add(scheduleType.Name, new RelayCommand(async () => { await UpdateContent(_dailyScheduleViewModel); }, true));
                        break;
                    case ScheduleType.WEEK:
                        ScheduleCommands.Add(scheduleType.Name, new RelayCommand(async () => { await UpdateContent(_weeklyScheduleViewModel); }, true));
                        break;
                    case ScheduleType.MONTH:
                        ScheduleCommands.Add(scheduleType.Name, new RelayCommand(async () => { await UpdateContent(_monthlyScheduleViewModel); }, true));
                        break;
                    case ScheduleType.YEAR:
                        ScheduleCommands.Add(scheduleType.Name, new RelayCommand(async () => { await UpdateContent(_yearlyScheduleViewModel); }, true));
                        break;
                    default:
                        throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(), scheduleType.Name));
                }
            }
        }

        private async Task UpdateContent(AbstractContentControlViewModel viewModel)
        {
            await SaveDirtyEntities();
            await viewModel.InitializeAsync();
            await viewModel.PostInitialize();
            CurrentViewModel = viewModel;
        }

        private async Task SaveDirtyEntities()
        {
            if (CurrentViewModel == null)
            {
                return;
            }

            foreach (var entity in CurrentViewModel.DirtyEntities.ToArray())
            {
                if (entity.CanAcceptChanges())
                {
                    await entity.AcceptChanges();
                }
            }
        }

        private void Subscribe()
        {
            Messenger.Default.Register<DirtyData>(
                this,
                (message) =>
                {
                    CurrentViewModel?.ReceiveMessage(message);
                });

            Messenger.Default.Register<RefreshData<Note>>(
                this,
                (message) =>
                {
                    switch (CurrentViewModel)
                    {
                        case DailySchedule ds:
                            _noteListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            break;
                        case ListViewModel<Note> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<ToDo<ScheduleSettingsSummary>>>(
                this,
                (message) =>
                {
                    switch (CurrentViewModel)
                    {
                        case ListViewModel<ToDo<ScheduleSettingsSummary>> lvm:
                            _dailyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<Habit<ScheduleSettingsSummary>>>(
               this,
               (message) =>
               {
                   switch (CurrentViewModel)
                   {
                       case ListViewModel<Habit<ScheduleSettingsSummary>> lvm:
                           _dailyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                           _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                           break;
                       default:
                           throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                   }
               });

            Messenger.Default.Register<RefreshData<Wish<ScheduleSettingsSummary>>>(
               this,
               (message) =>
               {
                   switch (CurrentViewModel)
                   {
                       case ListViewModel<Wish<ScheduleSettingsSummary>> lvm:
                           _dailyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                           _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                           break;
                       default:
                           throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                   }
               });

            Messenger.Default.Register<RefreshData<ToDo<ScheduleSettings>>>(
                this,
                (message) =>
                {
                    switch (CurrentViewModel)
                    {
                        case DailySchedule lvm:
                            _toDoListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<Habit<ScheduleSettings>>>(
                this,
                (message) =>
                {
                    switch (CurrentViewModel)
                    {
                        case DailySchedule lvm:
                            _habitListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                            break;
                        default:
                            throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                    }
                });

            Messenger.Default.Register<RefreshData<Wish<ScheduleSettings>>>(
               this,
               (message) =>
               {
                   switch (CurrentViewModel)
                   {
                       case DailySchedule lvm:
                           _wishListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                           _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                           break;
                       default:
                           throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                   }
               });

            Messenger.Default.Register<RefreshData<ScheduleSettings>>(
              this,
              (message) =>
              {
                  if (CurrentViewModel != _dailyScheduleViewModel)
                  {
                      throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
                  }

                  switch (message.OwnerType)
                  {
                      case OwnerTypes.ToDo:
                          _toDoListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          _yearlyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          break;
                      case OwnerTypes.Habit:
                          _habitListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          break;
                      case OwnerTypes.Wish:
                          _wishListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                          break;
                      default:
                          throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(message.OwnerType));
                  }
              });

            Messenger.Default.Register<RefreshData<Tracker>>(
              this,
              (message) =>
              {
                  if (CurrentViewModel == _dailyScheduleViewModel)
                  {
                      switch (message.OwnerType)
                      {
                          case OwnerTypes.ToDo:
                              _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              _yearlyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              break;
                          case OwnerTypes.Habit:
                              _weeklyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              _habitListViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              break;
                          default:
                              throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(message.OwnerType));
                      }

                      return;
                  }

                  if (CurrentViewModel == _weeklyScheduleViewModel)
                  {
                      switch (message.OwnerType)
                      {
                          case OwnerTypes.ToDo:
                              _dailyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              _yearlyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              break;
                          case OwnerTypes.Habit:
                              _dailyScheduleViewModel.RequestRefreshData(message.RangeStartDate, message.RangeFinishDate);
                              break;
                          default:
                              throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(message.OwnerType));
                      }

                      return;
                  }

                  throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(CurrentViewModel));
              });
        }
    }
}
