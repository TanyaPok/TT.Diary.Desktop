using System.Threading.Tasks;

namespace TT.Diary.Desktop.ViewModels.Commands.SaveCommands
{
    public interface IStoreKeeper<in T> where T : IStorable
    {
        bool CanAcceptChanges(T storable);
        void BeforeAcceptChanges(T storable);
        Task AfterAcceptChanges(T storable);
    }
}