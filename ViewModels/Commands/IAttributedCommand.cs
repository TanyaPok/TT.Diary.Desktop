using System.Windows.Input;

namespace TT.Diary.Desktop.ViewModels.Commands
{
    public interface IAttributedCommand : ICommand
    {
        string Name { get; }
        string ImgUrl { get; }
        void RaiseCanExecuteChanged();
    }
}