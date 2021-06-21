using GalaSoft.MvvmLight.Messaging;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Notification
{
    public class RefreshDataManager<T> : IManager<RefreshData<T>> where T : AbstractEntity
    {
        public void Dispose()
        {
        }

        public void Send(RefreshData<T> message)
        {
            Messenger.Default.Send<RefreshData<T>, Context>(message);
        }
    }
}