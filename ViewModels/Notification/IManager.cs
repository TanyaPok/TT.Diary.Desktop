using System;

namespace TT.Diary.Desktop.ViewModels.Notification
{
    public interface IManager<in T> : IDisposable where T : IMessage
    {
        void Send(T message);
    }
}
