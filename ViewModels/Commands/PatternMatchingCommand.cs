using System.Text.RegularExpressions;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace TT.Diary.Desktop.ViewModels.Commands
{
    public class PatternMatchingCommand : RelayCommand<TextCompositionEventArgs>
    {
        public PatternMatchingCommand(string pattern)
            : base
            (
                e =>
                {
                    var regex = new Regex(pattern);
                    e.Handled = !regex.IsMatch(e.Text);
                },
                e => true,
                true
            )
        {
        }
    }
}