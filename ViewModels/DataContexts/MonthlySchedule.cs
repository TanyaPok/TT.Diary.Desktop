using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class MonthlySchedule : ContentControlViewModel
    {
        public string Title
        {
            get
            {
                return "Monthly schedule";
            }
        }

        protected override Task LoadData(int UserId)
        {
            throw new System.NotImplementedException();
        }
    }
}
