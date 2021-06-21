namespace TT.Diary.Desktop.ViewModels.Interfaces
{
    public interface IWindowService
    {
        void Close();
        bool Activate();
        void ShowWindow(object context);
    }
}