using System;

namespace TT.Diary.Desktop.ViewModels.Common.Interfaces
{
    public interface IManager<T> : IDisposable where T : IMessage
    {
        void Send(T message);
    }
}
