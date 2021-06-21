using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.TimeManagement.UnscheduledSummaries
{
    public class UnscheduledWishSummary : UnscheduledItemSummary
    {
        public string Author { get; set; }
        public Rating Rating { get; set; }
    }
}