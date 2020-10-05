using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class YearlySchedule : ContentControlViewModel
    {
        public string Title
        {
            get
            {
                return "Yearly schedule";
            }
        }

        protected override Task LoadData(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
