using System.Windows;
using TT.Diary.Desktop.ViewModels.Interfaces;

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

        public void ShowWindow(object context)
        {
            var mainWindow = new MainWindow
            {
                DataContext = context
            };

            mainWindow.Show();
        }
    }
}