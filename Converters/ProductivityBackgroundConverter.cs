using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TT.Diary.Desktop.Converters
{
    public class ProductivityBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var productivity = (double)value;
            
            if (productivity >= 0 && productivity < 0.3)
            {
                return Brushes.Firebrick;
            }

            if (productivity >= 0.3 && productivity < 0.4)
            {
                return Brushes.OrangeRed;
            }

            if (productivity >= 0.4 && productivity < 0.7)
            {
                return Brushes.DarkOrange;
            }

            if (productivity >= 0.7 && productivity < 0.9)
            {
                return Brushes.OliveDrab;
            }

            if (productivity >= 0.9 && productivity <= 1)
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
