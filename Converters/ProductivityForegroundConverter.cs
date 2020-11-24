using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TT.Diary.Desktop.Converters
{
    public class ProductivityForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var productivity = (double)value;

            if (productivity >= 0 && productivity < 0.3 || productivity >= 0.9 && productivity <= 1)
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
