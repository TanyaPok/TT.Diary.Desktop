using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Common.Extensions;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractListItem : AbstractEntity
    {
        private string _description;
        public string Description
        {
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

        public AbstractListItem()
        {
            Description = string.Empty;
        }
    }
}
