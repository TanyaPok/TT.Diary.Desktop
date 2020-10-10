using System.Collections.Generic;
using System.Configuration;
using System.Windows.Input;
using TT.Diary.Desktop.Configs;
using System;
using GalaSoft.MvvmLight.Command;
using System.Net.Http;
using System.Net.Http.Headers;
using TT.Diary.Desktop.ViewModels.Common;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Linq;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.Common.Extensions;

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
        private readonly ListViewModel<Wish> _wishListViewModel;
        private readonly ListViewModel<ToDo> _toDoListViewModel;
        private readonly ListViewModel<Habit> _habitListViewModel;
        private readonly ListViewModel<Note> _noteListViewModel;

        internal static readonly HttpClient DiaryHttpClient;

        public string DictionaryTip { get { return "Show diary to-do lists"; } }
        public IList<Theme> Themes { get; private set; }
        public IList<ScheduleType> ScheduleTypesMenu { get; private set; }
        public IList<ListType> ListTypesMenu { get; private set; }
        public IDictionary<string, ICommand> ScheduleCommands { get; private set; }
        public IDictionary<string, ICommand> ListCommands { get; private set; }
        public ICommand ChangeThemeCommand { get; private set; }

        public User User { get; private set; }
        public string WindowTitle { get { return string.Format("{0}'s diary", User.Given_Name); } }

        private ContentControlViewModel _currentViewModel;
        public ContentControlViewModel CurrentViewModel
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
        }

        public Context(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));

            Themes = ConfigurationManager.GetSection(THEMES) as IList<Theme>;
            ScheduleTypesMenu = ConfigurationManager.GetSection(SCHEDULE_TYPES) as IList<ScheduleType>;
            ListTypesMenu = ConfigurationManager.GetSection(LIST_TYPES) as IList<ListType>;

            _dailyScheduleViewModel = new DailySchedule();
            _weeklyScheduleViewModel = new WeeklySchedule();
            _monthlyScheduleViewModel = new MonthlySchedule();
            _yearlyScheduleViewModel = new YearlySchedule();

            _wishListViewModel = new ListViewModel<Wish>(OperationContract.GET_WISH_LIST);
            _toDoListViewModel = new ListViewModel<ToDo>(OperationContract.GET_TODO_LIST);
            _habitListViewModel = new ListViewModel<Habit>(OperationContract.GET_HABITS);
            _noteListViewModel = new ListViewModel<Note>(OperationContract.GET_NOTES);

            ChangeThemeCommand = new RelayCommand(ChangeTheme, canExecute: () => true);
            SetScheduleCommands();
            SetListCommands();

            ChangeTheme();
            ScheduleCommands[ScheduleType.DAY].Execute(_dailyScheduleViewModel);
        }

        private void ChangeTheme()
        {
            if (Theme == null)
            {
                Theme = Themes.First();
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

        private async Task UpdateContent(ContentControlViewModel viewModel)
        {
            await viewModel.InitializeAsync(User.Id);
            CurrentViewModel = viewModel;
        }
    }
}
