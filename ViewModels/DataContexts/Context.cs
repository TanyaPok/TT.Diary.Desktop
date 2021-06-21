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
using TT.Diary.Desktop.ViewModels.Commands.InitializingCommands;
using TT.Diary.Desktop.ViewModels.Interfaces;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public partial class Context : ViewModelBase
    {
        private const string SCHEDULE_TYPES = "scheduleTypes";
        private const string LIST_TYPES = "listTypes";
        private const string THEMES = "themes";

        private readonly AbstractContentControlViewModel _dailyScheduleViewModel;
        private readonly AbstractContentControlViewModel _weeklyScheduleViewModel;
        private readonly AbstractContentControlViewModel _monthlyScheduleViewModel;
        private readonly AbstractContentControlViewModel _yearlyScheduleViewModel;
        private readonly AbstractContentControlViewModel _wishListViewModel;
        private readonly AbstractContentControlViewModel _toDoListViewModel;
        private readonly AbstractContentControlViewModel _appointmentListViewModel;
        private readonly AbstractContentControlViewModel _habitListViewModel;
        private readonly AbstractContentControlViewModel _noteListViewModel;

        private readonly User _user;
        private readonly IList<Theme> _themes;

        internal static readonly HttpClient DiaryHttpClient;

        private const string PRODUCTIVITY = "productivities";
        public static readonly IDictionary<ProductivityGradation, Productivity> Productivity;

        public string DictionaryTip => "Show diary to-do lists";
        public IList<ScheduleType> ScheduleTypesMenu { get; }
        public IList<ListType> ListTypesMenu { get; }
        public IDictionary<string, ICommand> ScheduleCommands { get; private set; }
        public IDictionary<string, ICommand> ListCommands { get; private set; }
        public ICommand ChangeThemeCommand { get; }

        public string WindowTitle => $"{_user.Given_Name}'s diary";

        private AbstractContentControlViewModel _currentViewModel;

        public AbstractContentControlViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => Set(ref _currentViewModel, value);
        }

        private Theme _theme;

        public Theme Theme
        {
            set => Set(ref _theme, value);
            get => _theme;
        }

        static Context()
        {
            var url = ConfigurationManager.ConnectionStrings["WebApiUrl"].ConnectionString;
            DiaryHttpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            DiaryHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var productivity = ConfigurationManager.GetSection(PRODUCTIVITY) as IList<Productivity>;
            Productivity = new Dictionary<ProductivityGradation, Productivity>();
            foreach (var value in productivity)
            {
                Productivity.Add(value.Gradation, value);
            }
        }

        public Context(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));

            _themes = ConfigurationManager.GetSection(THEMES) as IList<Theme>;
            ScheduleTypesMenu = ConfigurationManager.GetSection(SCHEDULE_TYPES) as IList<ScheduleType>;
            ListTypesMenu = ConfigurationManager.GetSection(LIST_TYPES) as IList<ListType>;

            _dailyScheduleViewModel = new DailySchedule(_user.Id);
            _weeklyScheduleViewModel = new WeeklySchedule(_user.Id);
            _monthlyScheduleViewModel = new MonthlySchedule(_user.Id);
            _yearlyScheduleViewModel = new YearlySchedule(_user.Id);

            _wishListViewModel =
                new ListViewModel<Wish<ScheduleSettingsSummary>>(ServiceOperationContract.GetWishList, _user.Id);
            _toDoListViewModel =
                new ListViewModel<ToDo<ScheduleSettingsSummary>>(ServiceOperationContract.GetTodoList, _user.Id);
            _appointmentListViewModel =
                new ListViewModel<Appointment<ScheduleSettingsSummary>>(ServiceOperationContract.GetAppointments,
                    _user.Id);
            _habitListViewModel =
                new ListViewModel<Habit<ScheduleSettingsSummary>>(ServiceOperationContract.GetHabits, _user.Id);
            _noteListViewModel = new ListViewModel<Note>(ServiceOperationContract.GetNotes, _user.Id);

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
            if (ScheduleCommands[ScheduleType.Day].CanExecute(null))
            {
                ScheduleCommands[ScheduleType.Day].Execute(null);
            }
        }

        private void SetDefaultTheme()
        {
            Theme = _themes.First();
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
                    case Theme.Tea:
                        Theme = _themes.Single(t => t.Name == Theme.Coffee);
                        break;
                    case Theme.Coffee:
                        Theme = _themes.Single(t => t.Name == Theme.Tea);
                        break;
                    default:
                        throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(),
                            Theme.Name));
                }
            }

            ((IResourceService) App.Current).ChangeTheme(Theme.Source);
        }

        private void SetListCommands()
        {
            ListCommands = new Dictionary<string, ICommand>();
            foreach (var listType in ListTypesMenu)
            {
                switch (listType.Name)
                {
                    case ListType.WishList:
                        ListCommands.Add(listType.Name,
                            new RelayCommand(async () => { await UpdateContent(_wishListViewModel); }, true));
                        break;
                    case ListType.TodoList:
                        ListCommands.Add(listType.Name,
                            new RelayCommand(async () => { await UpdateContent(_toDoListViewModel); }, true));
                        break;
                    case ListType.Appointments:
                        ListCommands.Add(listType.Name,
                            new RelayCommand(async () => { await UpdateContent(_appointmentListViewModel); }, true));
                        break;
                    case ListType.Habits:
                        ListCommands.Add(listType.Name,
                            new RelayCommand(async () => { await UpdateContent(_habitListViewModel); }, true));
                        break;
                    case ListType.Notes:
                        ListCommands.Add(listType.Name,
                            new RelayCommand(async () => { await UpdateContent(_noteListViewModel); }, true));
                        break;
                    default:
                        throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(),
                            listType.Name));
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
                    case ScheduleType.Day:
                        ScheduleCommands.Add(scheduleType.Name,
                            new RelayCommand(async () => { await UpdateContent(_dailyScheduleViewModel); }, true));
                        break;
                    case ScheduleType.Week:
                        ScheduleCommands.Add(scheduleType.Name,
                            new RelayCommand(async () => { await UpdateContent(_weeklyScheduleViewModel); }, true));
                        break;
                    case ScheduleType.Month:
                        ScheduleCommands.Add(scheduleType.Name,
                            new RelayCommand(async () => { await UpdateContent(_monthlyScheduleViewModel); }, true));
                        break;
                    case ScheduleType.Year:
                        ScheduleCommands.Add(scheduleType.Name,
                            new RelayCommand(async () => { await UpdateContent(_yearlyScheduleViewModel); }, true));
                        break;
                    default:
                        throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(),
                            scheduleType.Name));
                }
            }
        }

        private async Task UpdateContent(AbstractContentControlViewModel viewModel)
        {
            if (CurrentViewModel != null)
            {
                if (CurrentViewModel.IsAutosaveCapable)
                {
                    await CurrentViewModel.SaveDirtyEntitiesAsync();
                }

                if (CurrentViewModel is ICreator creator)
                {
                    creator.RemoveNewNotSavedEntities();
                }
            }

            await viewModel.InitializeAsync();
            CurrentViewModel = viewModel;
        }

        private void Subscribe()
        {
            RegisterDirtyDataMessage();
            RegisterRefreshNoteMessage();
            RegisterRefreshToDoMessage();
            RegisterRefreshAppointmentMessage();
            RegisterRefreshHabitMessage();
            RegisterRefreshWishMessage();
            RegisterRefreshScheduleSettingsMessage();
            RegisterRefreshTrackerMessage();
        }
    }
}