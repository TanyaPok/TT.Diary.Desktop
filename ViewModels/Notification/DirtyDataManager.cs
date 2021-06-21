using GalaSoft.MvvmLight.Messaging;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Notification
{
    public class DirtyDataManager : IManager<DirtyData>
    {
        public void Dispose()
        {
        }

        public void Send(DirtyData message)
        {
            Messenger.Default.Send<DirtyData, Context>(message);
        }
    }
}