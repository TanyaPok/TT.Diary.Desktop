namespace TT.Diary.Desktop.ViewModel
{
    public abstract class ControlModel : NotifyPropertyChanged
    {
        private bool _isVisisble;
        public bool IsVisible
        {
            get
            {
                return _isVisisble;
            }
            set
            {
                if (value == _isVisisble)
                    return;

                _isVisisble = value;
                OnPropertyChanged();
            }
        }

        public abstract string Title { get; }
    }
}
