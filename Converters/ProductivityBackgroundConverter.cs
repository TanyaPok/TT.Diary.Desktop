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
            var productivity = (double)value;

            if (productivity >= Context.Productivities[ProductivityGradation.Horrible].Begin
                && productivity < Context.Productivities[ProductivityGradation.Horrible].End)
            {
                return Brushes.Firebrick;
            }

            if (productivity >= Context.Productivities[ProductivityGradation.Bad].Begin
                && productivity < Context.Productivities[ProductivityGradation.Bad].End)
            {
                return Brushes.OrangeRed;
            }

            if (productivity >= Context.Productivities[ProductivityGradation.Normal].Begin
                && productivity < Context.Productivities[ProductivityGradation.Normal].End)
            {
                return Brushes.DarkOrange;
            }

            if (productivity >= Context.Productivities[ProductivityGradation.Good].Begin
                && productivity < Context.Productivities[ProductivityGradation.Good].End)
            {
                return Brushes.OliveDrab;
            }

            if (productivity >= Context.Productivities[ProductivityGradation.Excellent].Begin
                && productivity <= Context.Productivities[ProductivityGradation.Excellent].End)
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
