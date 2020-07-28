namespace TT.Diary.Desktop.ViewModel
{
    public interface IWindowService
    {
        void Close();
        bool Activate();
        void ShowWindow(object context);
        int ShowMessageBox(string message, string caption);
    }
}
