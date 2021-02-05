using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var CollectionChanged = this.CollectionChanged;

            if (CollectionChanged == null)
            {
                return;
            }

            foreach (NotifyCollectionChangedEventHandler notificationHandler in CollectionChanged.GetInvocationList())
            {
                var dispObj = notificationHandler.Target as DispatcherObject;

                if (dispObj == null)
                {
                    notificationHandler.Invoke(this, e);
                    continue;
                }

                var dispatcher = dispObj.Dispatcher;

                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.BeginInvoke((Action)(() => notificationHandler.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))), DispatcherPriority.DataBind);
                    continue;
                }

                notificationHandler.Invoke(this, e);
            }
        }
    }
}
