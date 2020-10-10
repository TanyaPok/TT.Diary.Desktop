using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class ContentControlViewModel : ViewModelBase
    {
        protected readonly string DATE_FORMAT = "yyyy-MM-dd";

        protected readonly string LISTVIEWMODEL_NOTE = "ListViewModel.Note";

        protected readonly string DAILYSCHEDULE_NOTE = "DailySchedule.Note";

        protected readonly string ADD_LIST_ITEM = "Add {0}";

        protected readonly string IMG_URL_LIST_ITEM = "pack://application:,,,/Images/Toolbar/add.png";

        protected bool IsDataLoaded { get; set; }

        internal async Task InitializeAsync(int userId)
        {
            if (!IsDataLoaded)
            {
                await LoadData(userId);
                IsDataLoaded = true;
            }
        }

        internal void Initialize(int userId)
        {
            if (!IsDataLoaded)
            {
                LoadData(userId);
                IsDataLoaded = true;
            }
        }

        protected abstract Task LoadData(int userId);

        public ObservableCollection<Command> Commands { get; protected set; }
    }
}