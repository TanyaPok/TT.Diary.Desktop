using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.Views.Controls.TimeManagement
{
    public interface IScheduledListSource
    {
        IAttributedCommand InitializeItemCreateCommand { get; }
        IAttributedCommand ItemSaveCommand { get; }
        IAttributedCommand ItemDeleteCommand { get; }
        IAttributedCommand CompleteItemCommand { get; }
        IAttributedCommand RescheduleItemCommand { get; }
        DateRange DateRange { get; }
    }
}
