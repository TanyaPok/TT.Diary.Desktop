using System.Threading.Tasks;

namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IEntityCommands
    {
        IAttributedCommand SaveCommand { get; }

        bool IsChanged { get; }

        bool CanAcceptChanges();

        Task AcceptChanges();
    }
}
