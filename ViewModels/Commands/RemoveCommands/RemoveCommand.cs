using System;
using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Commands.RemoveCommands
{
    public class RemoveCommand<T, TP> : RelayCommand<T>, IAttributedCommand where T : IRemovable where TP : IRemover<T>
    {
        private readonly string _entityName;
        public string Name => $"Remove {_entityName}";
        public string ImgUrl => "pack://application:,,,/Images/Toolbar/remove.png";

        public RemoveCommand(string entityName = "")
            : base((T removable) => throw new NotImplementedException(), (T removable) => false)
        {
            _entityName = entityName;
        }

        public RemoveCommand(TP remover, string entityName = "", bool keepTargetAlive = false)
            : base
            (
                async (T removable) =>
                {
                    if (removable.Id != default)
                    {
                        await Endpoint.RemoveAsync(removable.GetRemoveRequest());
                    }

                    remover.Remove(removable);
                },
                (T removable) => 
                    removable != null && remover.CanRemove(removable) && removable.CanBeRemoved(),
                keepTargetAlive
            )
        {
            _entityName = entityName;
        }
    }
}