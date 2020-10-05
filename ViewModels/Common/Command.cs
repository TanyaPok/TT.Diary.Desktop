using GalaSoft.MvvmLight.Command;
using System;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public class Command : RelayCommand
    {
        public string Name { get; private set; }

        public string ImgUrl { get; private set; }

        public Command(string name, string imgUrl, Action action) : base(action)
        {
            SetProperties(name, imgUrl);
        }

        public Command(string name, string imgUrl, Action action, Func<bool> canExecute) : base(action, canExecute)
        {
            SetProperties(name, imgUrl);
        }

        private void SetProperties(string name, string imgUrl)
        {
            Name = name;
            ImgUrl = imgUrl;
        }
    }
}
