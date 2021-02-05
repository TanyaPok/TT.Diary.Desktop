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
            var productivity = (double)value;

            if (productivity >= Context.Productivities[ProductivityGradation.Horrible].Begin
                && productivity < Context.Productivities[ProductivityGradation.Horrible].End ||
                productivity >= Context.Productivities[ProductivityGradation.Excellent].Begin
                && productivity <= Context.Productivities[ProductivityGradation.Excellent].End)
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
