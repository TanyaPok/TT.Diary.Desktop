namespace TT.Diary.Desktop.ViewModels.Notification
{
    public interface IPublisher<in M> where M : IMessage
    {
        void Notify(M message);
    }
}
