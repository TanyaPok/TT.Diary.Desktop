using TT.Diary.Desktop.ViewModels.Commands.RemoveCommands;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Extensions;
using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public abstract class AbstractListItem : AbstractEntity, IRemovable
    {
        private string _description;

        [TrackChange]
        public string Description
        {
            get => _description;
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

        protected AbstractListItem()
        {
            Description = string.Empty;
        }

        public override void AfterAcceptChanges()
        {
            base.AfterAcceptChanges();
            Notify();
        }

        #region implementation IRemovable

        public abstract bool CanBeRemoved();
        public abstract Request GetRemoveRequest();

        #endregion

        public abstract void Notify();
    }
}