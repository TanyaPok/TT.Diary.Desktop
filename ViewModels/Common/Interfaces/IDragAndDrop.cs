using System.Windows.Input;

namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IDragAndDrop
    {
       ICommand MouseMoveObjectCommand { get; }

       ICommand DragEnterOverLeaveObjectCommand { get; }

       ICommand DropObjectCommand { get; }
    }
}
