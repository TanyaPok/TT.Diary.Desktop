using System;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Note : AbstractListItem
    {
        internal int UserId { get; set; }

        public DateTime ScheduleDate { get; set; }

        protected override string RemoveOperationContract
        {
            get
            {
                return OperationContract.NOTE;
            }
        }

        internal override bool CanRemove()
        {
            return true;
        }

        internal override async void SaveAsync()
        {
            var note = new { Id, Description, ScheduleDate = ScheduleDate, UserId };
            HttpResponseMessage response = null;
            try
            {
                response = (Id == 0) ?
                    await Context.DiaryHttpClient.PostAsJsonAsync(OperationContract.NOTE, note) :
                    await Context.DiaryHttpClient.PutAsJsonAsync(OperationContract.NOTE, note);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var errorMessageFormat = (Id == 0) ? ErrorMessages.Add.GetDescription() : ErrorMessages.Edit.GetDescription();
                    throw new Exception(string.Format(errorMessageFormat, "Note " + Description, errorMessage));
                }

                if (Id == 0)
                {
                    Id = await response.Content.ReadAsAsync<int>();
                }

                SendDiaryNotificationMessage(ScheduleDate);
            }
            finally
            {
                response.Dispose();
            }
        }

        internal override async Task<bool> RemoveAsync()
        {
            var result = await base.RemoveAsync();

            if (result)
            {
                SendDiaryNotificationMessage(ScheduleDate);
            }

            return result;
        }
    }
}
