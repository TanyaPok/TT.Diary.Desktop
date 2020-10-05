using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.DataContexts
{
    public class DailySchedule : ContentControlViewModel
    {
        public string Title
        {
            get
            {
                return "Daily schedule for";
            }
        }

        protected override Task LoadData(int UserId)
        {
            return Task.CompletedTask;
        }
    }
}
