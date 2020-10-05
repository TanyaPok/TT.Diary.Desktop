using System;
using System.Net.Http;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class ToDo : AbstractListItem
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return OperationContract.TODO;
            }
        }

        public ToDo()
        {
            Description = "new to-do";
        }

        internal override async void SaveAsync()
        {
            var todo = new { Id, Description, CategoryId = ParentId };
            HttpResponseMessage response = null;
            try
            {
                response = (Id == 0) ?
                    await Context.DiaryHttpClient.PostAsJsonAsync(OperationContract.TODO, todo) :
                    await Context.DiaryHttpClient.PutAsJsonAsync(OperationContract.TODO, todo);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var errorMessageFormat = (Id == 0) ? ErrorMessages.Add.GetDescription() : ErrorMessages.Edit.GetDescription();
                    throw new Exception(string.Format(errorMessageFormat, "To-do " + Description, errorMessage));
                }

                if (Id == 0)
                {
                    Id = await response.Content.ReadAsAsync<int>();
                }
            }
            finally
            {
                response.Dispose();
            }
        }
    }
}
