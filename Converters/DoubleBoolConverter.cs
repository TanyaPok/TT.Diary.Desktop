using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class DoubleBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double result))
            {
                return result > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value.ToString(), out bool result))
            {
                return result ? 1.0 : 0.0;
            }

            return 0.0;
        }
    }
}
