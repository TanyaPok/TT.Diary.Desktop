namespace TT.Diary.Desktop.ViewModels.Commands.RemoveCommands
{
    public interface IRemover<in T> where T : IRemovable
    {
        bool CanRemove(T entity);
        void Remove(T entity);
    }
}