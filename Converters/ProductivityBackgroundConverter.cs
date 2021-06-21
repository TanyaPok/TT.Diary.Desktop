using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TT.Diary.Desktop.Configs;
using TT.Diary.Desktop.ViewModels.DataContexts;

namespace TT.Diary.Desktop.Converters
{
    public class ProductivityBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Transparent;
            var productivity = (double) value;

            if (productivity >= Context.Productivity[ProductivityGradation.Horrible].Begin
                && productivity < Context.Productivity[ProductivityGradation.Horrible].End)
            {
                return Brushes.Firebrick;
            }

            if (productivity >= Context.Productivity[ProductivityGradation.Bad].Begin
                && productivity < Context.Productivity[ProductivityGradation.Bad].End)
            {
                return Brushes.OrangeRed;
            }

            if (productivity >= Context.Productivity[ProductivityGradation.Normal].Begin
                && productivity < Context.Productivity[ProductivityGradation.Normal].End)
            {
                return Brushes.DarkOrange;
            }

            if (productivity >= Context.Productivity[ProductivityGradation.Good].Begin
                && productivity < Context.Productivity[ProductivityGradation.Good].End)
            {
                return Brushes.OliveDrab;
            }

            if (productivity >= Context.Productivity[ProductivityGradation.Excellent].Begin
                && productivity <= Context.Productivity[ProductivityGradation.Excellent].End)
            {
                return Brushes.DarkGreen;
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}