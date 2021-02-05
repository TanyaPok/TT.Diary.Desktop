namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IPublisher<M> where M : IMessage
    {
        void Notify(M message);
    }
}
