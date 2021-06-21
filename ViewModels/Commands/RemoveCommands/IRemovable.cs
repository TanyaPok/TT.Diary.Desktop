using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Commands.RemoveCommands
{
    public interface IRemovable : IEntity
    {
        bool CanBeRemoved();
        Request GetRemoveRequest();
    }
}