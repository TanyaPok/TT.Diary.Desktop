using System.Windows.Input;

namespace TT.Diary.Desktop.ViewModels.Commands
{
    public interface IDragAndDrop
    {
       ICommand MouseMoveObjectCommand { get; }

       ICommand DragEnterOverLeaveObjectCommand { get; }

       ICommand DropObjectCommand { get; }
    }
}
