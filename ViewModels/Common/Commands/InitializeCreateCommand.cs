using GalaSoft.MvvmLight.CommandWpf;
using System;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.Common.Commands
{
    public class InitializeCreateCommand : RelayCommand, IAttributedCommand
    {
        private readonly string _entityName;

        public string Name => string.Format("Add {0}", _entityName);

        public string ImgUrl => "pack://application:,,,/Images/Toolbar/add.png";

        public InitializeCreateCommand(Action execute, Func<bool> canExecute, string entityName = "", bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
            _entityName = entityName;
        }
    }
}
