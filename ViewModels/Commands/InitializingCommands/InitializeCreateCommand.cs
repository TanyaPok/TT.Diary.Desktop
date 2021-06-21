using GalaSoft.MvvmLight.CommandWpf;
using System;

namespace TT.Diary.Desktop.ViewModels.Commands.InitializingCommands
{
    public class InitializeCreateCommand : RelayCommand, IAttributedCommand
    {
        private readonly string _entityName;
        public string Name => $"Add {_entityName}";
        public string ImgUrl => "pack://application:,,,/Images/Toolbar/add.png";

        public InitializeCreateCommand(Action execute, Func<bool> canExecute, string entityName = "",
            bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
            _entityName = entityName;
        }
    }
}