using System.Collections.Generic;
using System.Configuration;
using System.Windows.Input;
using TT.Diary.Desktop.Configs;
using System;
using GalaSoft.MvvmLight.Command;
using TT.Diary.Desktop.ViewModel.Schedules;
using System.Net.Http;
using System.Net.Http.Headers;
using TT.Diary.Desktop.ViewModel.Lists;
using System.Linq;

namespace TT.Diary.Desktop.ViewModel
{
    public partial class Context : NotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient;
        private readonly string UNEXPECTED_SCHEDULE_TYPE = "Unexpected schedule type {0}.";

        public string DictionaryTip { get; private set; }
        public IList<ScheduleType> ScheduleTypes { get; private set; }
        public IList<ListType> ListTypes { get; private set; }
        public IDictionary<string, ICommand> ShowScheduleCommands { get; private set; }
        public IDictionary<string, ICommand> ShowListCommands { get; private set; }

        public DailySchedule DailyScheduleModel { get; private set; }
        public WeeklySchedule WeeklyScheduleModel { get; private set; }
        public MonthlySchedule MonthlyScheduleModel { get; private set; }
        public YearlySchedule YearlyScheduleModel { get; private set; }

        public ListModel CurrentListModel { get; private set; }

        public User User { get; private set; }
        public string Title { get { return string.Format("{0}'s diary", User.Given_Name); } }

        static Context()
        {
            var url = ConfigurationManager.ConnectionStrings["WebApiUrl"].ConnectionString;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Context(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));

            ScheduleTypes = ConfigurationManager.GetSection("scheduleTypes") as IList<ScheduleType>;
            ListTypes = ConfigurationManager.GetSection("listTypes") as IList<ListType>;
            DictionaryTip = ConfigurationManager.AppSettings["dictionaryTip"];

            DailyScheduleModel = new DailySchedule();
            WeeklyScheduleModel = new WeeklySchedule();
            MonthlyScheduleModel = new MonthlySchedule();
            YearlyScheduleModel = new YearlySchedule();

            CurrentListModel = new ListModel();

            CreateScheduleCommands();
            CrateListCommands();

            DailyScheduleModel.IsVisible = true;
        }

        private void CrateListCommands()
        {
            ShowListCommands = new Dictionary<string, ICommand>();
            foreach (var listType in ListTypes)
            {
                switch (listType.Name)
                {
                    case "Wish list":
                        ShowListCommands.Add(listType.Name, new RelayCommand(() =>
                        {
                            CollapseWorkspace();
                            CurrentListModel.IsVisible = true;
                        }, true));
                        break;
                    default:
                        throw new ArgumentException(string.Format(UNEXPECTED_SCHEDULE_TYPE, listType.Name));
                }
            }
        }

        private void CreateScheduleCommands()
        {
            ShowScheduleCommands = new Dictionary<string, ICommand>();
            foreach (var scheduleType in ScheduleTypes)
            {
                switch (scheduleType.Name)
                {
                    case "Day":
                        ShowScheduleCommands.Add(scheduleType.Name, new RelayCommand(() =>
                        {
                            CollapseWorkspace();
                            DailyScheduleModel.IsVisible = true;
                        }, true));
                        break;
                    case "Week":
                        ShowScheduleCommands.Add(scheduleType.Name, new RelayCommand(() =>
                        {
                            CollapseWorkspace();
                            WeeklyScheduleModel.IsVisible = true;
                        }, true));
                        break;
                    case "Month":
                        ShowScheduleCommands.Add(scheduleType.Name, new RelayCommand(() =>
                        {
                            CollapseWorkspace();
                            MonthlyScheduleModel.IsVisible = true;
                        }, true));
                        break;
                    case "Year":
                        ShowScheduleCommands.Add(scheduleType.Name, new RelayCommand(() =>
                        {
                            CollapseWorkspace();
                            YearlyScheduleModel.IsVisible = true;
                        }, true));
                        break;
                    default:
                        throw new ArgumentException(string.Format(UNEXPECTED_SCHEDULE_TYPE, scheduleType.Name));
                }
            }
        }

        private void CollapseWorkspace()
        {
            CurrentListModel.IsVisible =
            DailyScheduleModel.IsVisible =
            WeeklyScheduleModel.IsVisible =
            MonthlyScheduleModel.IsVisible =
            YearlyScheduleModel.IsVisible = false;
        }
    }
}
