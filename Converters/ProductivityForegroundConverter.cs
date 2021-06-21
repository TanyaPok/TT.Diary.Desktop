using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TT.Diary.Desktop.Configs;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.Converters
{
    public class ProductivityForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Black;
            var productivity = (double) value;

            if (productivity >= Context.Productivity[ProductivityGradation.Horrible].Begin
                && productivity < Context.Productivity[ProductivityGradation.Horrible].End ||
                productivity >= Context.Productivity[ProductivityGradation.Excellent].Begin
                && productivity <= Context.Productivity[ProductivityGradation.Excellent].End)
            {
                return Brushes.White;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}