using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractListItem : ObservableObjectWithNotifyDataErrorInfo, IMessaging
    {
        protected abstract string RemoveOperationContract { get; }

        public int Id { get; set; }

        public DateTime? ScheduledStartDate { get; set; }

        public DateTime? ScheduledCompletionDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public int ParentId { get; internal set; }

        public bool IsComplited
        {
            get
            {
                return CompletionDate != null;
            }
        }

        private string _description;
        public string Description {
            get
            {
                return _description;
            }
            set
            {
                ClearErrors(nameof(Description));
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    AddError(nameof(Description), ValidationMessages.EmptyDescription.GetDescription());
                }

                Set(ref _description, value);
            }
        }
        public RelayCommand SaveCommand { get; set; }

        public string SenderPath { get; internal set; }

        public AbstractListItem()
        {
            PropertyChanged += AbstractListItem_PropertyChanged;
        }

        ~AbstractListItem()
        {
            PropertyChanged -= AbstractListItem_PropertyChanged;
        }

        internal virtual bool CanSave()
        {
            return !HasErrors;
        }

        internal abstract void SaveAsync();

        internal virtual bool CanRemove()
        {
            return !ScheduledStartDate.HasValue;
        }

        internal virtual async Task<bool> RemoveAsync()
        {
            if (Id == 0)
                return true;

            var requestUri = string.Format(OperationContract.REQUEST_FORMAT, RemoveOperationContract, Id);
            using (var response = await Context.DiaryHttpClient.DeleteAsync(requestUri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception(string.Format(ErrorMessages.Remove.GetDescription(), Description, errorMessage));
                }

                TrySendDiaryNotificationMessage();
            }

            return true;
        }

        protected bool TrySendDiaryNotificationMessage()
        {
            if (ScheduledStartDate != null)
            {
                Messenger.Default.Send(new DiaryNotificationMessage(SenderPath, ScheduledStartDate, ScheduledCompletionDate, CompletionDate));
                return true;
            }

            return false;
        }

        private void AbstractListItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (SaveCommand != null && SaveCommand.CanExecute(this))
            {
                SaveCommand.Execute(this);
            }
        }
    }
}
