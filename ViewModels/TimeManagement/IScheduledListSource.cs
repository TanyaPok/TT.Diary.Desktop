using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public interface IScheduledListSource
    {
        IAttributedCommand InitializeItemCreateCommand { get; }
        IAttributedCommand ItemSaveCommand { get; }
        IAttributedCommand ItemDeleteCommand { get; }
        IAttributedCommand ItemCompleteCommand { get; }
        IAttributedCommand ItemRescheduleCommand { get; }
        DateRange DateRange { get; }
    }
}