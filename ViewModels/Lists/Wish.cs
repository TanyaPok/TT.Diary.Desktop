using System;
using System.Net.Http;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public enum Rating
    {
        Empty,
        Trash,
        NotSoBad,
        Normal,
        Good,
        Fire
    }

    public class Wish : AbstractListItem
    {
        private Rating _rating;
        public Rating Rating
        {
            set
            {
                Set(ref _rating, value);
            }
            get
            {
                return _rating;
            }
        }

        private string _author;
        public string Author
        {
            set
            {
                Set(ref _author, value);
            }
            get
            {
                return _author;
            }
        }

        protected override string RemoveOperationContract
        {
            get
            {
                return OperationContract.WISH;
            }
        }

        public Wish()
        {
            Description = "my new wish";
        }

        internal override async void SaveAsync()
        {
            var wish = new { Id, Description, CategoryId = ParentId, Author, Rating };
            HttpResponseMessage response = null;
            try
            {
                response = (Id == 0) ? 
                    await Context.DiaryHttpClient.PostAsJsonAsync(OperationContract.WISH, wish) : 
                    await Context.DiaryHttpClient.PutAsJsonAsync(OperationContract.WISH, wish);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var errorMessageFormat = (Id == 0) ? ErrorMessages.Add.GetDescription() : ErrorMessages.Edit.GetDescription();
                    throw new Exception(string.Format(errorMessageFormat, "Wish " + Description, errorMessage));
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