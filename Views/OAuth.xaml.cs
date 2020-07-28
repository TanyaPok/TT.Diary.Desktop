using System.Windows;
using TT.Diary.Desktop.ViewModel;

namespace TT.Diary.Desktop.Views
{
    /// <summary>
    /// Interaction logic for OAuth.xaml
    /// </summary>
    public partial class OAuth : Window, IWindowService
    {
        public OAuth()
        {
            InitializeComponent();
        }

        public int ShowMessageBox(string message, string caption)
        {
            return (int)MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        public void ShowWindow(object context)
        {
            MainWindow mainWindow = new MainWindow
            {
                DataContext = context
            };
            mainWindow.Show();
        }
    }
}
