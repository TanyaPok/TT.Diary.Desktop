using GalaSoft.MvvmLight.CommandWpf;
using System;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.Common.Commands
{
    public class CompleteCommand : RelayCommand, IAttributedCommand
    {
        public string Name => "Complete";

        public string ImgUrl => "pack://application:,,,/Images/Toolbar/done.png";

        public CompleteCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
        }
    }
}