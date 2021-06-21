using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace TT.Diary.Desktop.ViewModels.Common
{
    /// <summary>
    /// Using ObservableCollection in a multi threaded environment
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MtObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            foreach (NotifyCollectionChangedEventHandler notificationHandler in CollectionChanged.GetInvocationList())
            {
                var obj = notificationHandler.Target as DispatcherObject;

                if (obj == null)
                {
                    notificationHandler.Invoke(this, e);
                    continue;
                }

                var dispatcher = obj.Dispatcher;

                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.BeginInvoke(
                        (Action) (() => notificationHandler.Invoke(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                        DispatcherPriority.DataBind);
                    continue;
                }

                notificationHandler.Invoke(this, e);
            }
        }
    }
}