using GalaSoft.MvvmLight.CommandWpf;
using System;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.Common.Commands
{
    public class CompleteCommand<T> : RelayCommand<T>, IAttributedCommand where T : ObservableObjectWithNotifyDataErrorInfo
    {
        public string Name => "Complete";

        public string ImgUrl => "pack://application:,,,/Images/Toolbar/done.png";

        public CompleteCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
        }
    }
}