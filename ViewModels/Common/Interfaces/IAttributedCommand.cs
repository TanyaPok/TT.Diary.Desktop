using System.Windows.Input;

namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IAttributedCommand : ICommand
    {
        string Name { get; }

        string ImgUrl { get; }

        void RaiseCanExecuteChanged();
    }
}
