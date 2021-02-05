using GalaSoft.MvvmLight.CommandWpf;
using System;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.Common.Commands
{
    public class SaveCommand : RelayCommand, IAttributedCommand
    {
        public string Name => "Save";

        public string ImgUrl => "pack://application:,,,/Images/Toolbar/save.png";

        public SaveCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
        }
    }
}