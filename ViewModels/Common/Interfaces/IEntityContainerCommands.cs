namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IEntityContainerCommands
    {
        IAttributedCommand InitializeNestedEntityCreateCommand { get; }

        IAttributedCommand InitializeNestedEntityUpdateCommand { get; }

        IAttributedCommand NestedEntityDeleteCommand { get; }
    }
}
