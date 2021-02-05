using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class WeeklySchedule : AbstractContentControlViewModel
    {
        public string Title
        {
            get
            {
                return "Weekly schedule";
            }
        }

        protected override Task LoadData()
        {
            throw new System.NotImplementedException();
        }

        protected override async Task DataSetting()
        {
            throw new System.NotImplementedException();
        }
    }
}
