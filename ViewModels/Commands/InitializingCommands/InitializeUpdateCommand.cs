using GalaSoft.MvvmLight.CommandWpf;
using System;

namespace TT.Diary.Desktop.ViewModels.Commands.InitializingCommands
{
    public class InitializeUpdateCommand : RelayCommand, IAttributedCommand
    {
        private readonly string _entityName;
        public string Name => $"Edit {_entityName}";
        public string ImgUrl => "pack://application:,,,/Images/Toolbar/edit.png";

        public InitializeUpdateCommand(Action execute, Func<bool> canExecute, string entityName = "",
            bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
            _entityName = entityName;
        }
    }
}