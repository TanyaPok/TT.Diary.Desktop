using System;
using System.Net.Http;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Habit : AbstractListItem
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return OperationContract.HABIT;
            }
        }

        private uint? _amount;
        public uint? Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                Set(ref _amount, value);
            }
        }

        public Habit()
        {
            Description = "new habit: drink water (glasses)";
        }

        internal override async void SaveAsync()
        {
            var habit = new { Id, Description, CategoryId = ParentId, Amount };
            HttpResponseMessage response = null;
            try
            {
                response = (Id == 0) ?
                    await Context.DiaryHttpClient.PostAsJsonAsync(OperationContract.HABIT, habit) :
                    await Context.DiaryHttpClient.PutAsJsonAsync(OperationContract.HABIT, habit);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var errorMessageFormat = (Id == 0) ? ErrorMessages.Add.GetDescription() : ErrorMessages.Edit.GetDescription();
                    throw new Exception(string.Format(errorMessageFormat, "Habit " + Description, errorMessage));
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
