using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class WeeklySchedule : ContentControlViewModel
    {
        public string Title
        {
            get
            {
                return "Weekly schedule";
            }
        }

        protected override Task LoadData(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
