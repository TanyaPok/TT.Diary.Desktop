using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Commands.SaveCommands
{
    public interface IStorable : IEntity
    {
        bool CanAcceptChanges();
        void AfterAcceptChanges();
        Request GetCreateRequest();
        Request GetUpdateRequest();
    }
}