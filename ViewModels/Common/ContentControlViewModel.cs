using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class ContentControlViewModel : ViewModelBase
    {
        private bool _isDataLoaded;
        
        internal async Task Initialize(int userId)
        {
            if (!_isDataLoaded)
            {
                await LoadData(userId);
                _isDataLoaded = true;
            }
        }

        protected abstract Task LoadData(int userId);

        public ObservableCollection<Command> Commands { get; protected set; }
    }
}