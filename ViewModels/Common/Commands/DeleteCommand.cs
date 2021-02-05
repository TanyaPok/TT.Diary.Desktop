using GalaSoft.MvvmLight.CommandWpf;
using System;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.Common.Commands
{
    public class DeleteCommand<T> : RelayCommand<T>, IAttributedCommand where T : ObservableObjectWithNotifyDataErrorInfo
    {
        private readonly string _entityName;

        public string Name => string.Format("Remove {0}", _entityName);

        public string ImgUrl => "pack://application:,,,/Images/Toolbar/remove.png";

        public DeleteCommand(Action<T> execute, Func<T, bool> canExecute, string entityName = "", bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
            _entityName = entityName;
        }
    }
}