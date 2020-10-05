using System;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class Note : AbstractListItem
    {
        protected override string RemoveOperationContract
        {
            get
            {
                return OperationContract.NOTE;
            }
        }

        public DateTime CreationDate { get; set; }

        internal override void SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
