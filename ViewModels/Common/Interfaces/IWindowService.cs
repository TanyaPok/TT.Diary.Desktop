namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IWindowService
    {
        void Close();

        bool Activate();

        void ShowWindow(object context);
    }
}
