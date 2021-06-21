using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class EnumInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Enum current)
            {
                return Visibility.Collapsed;
            }

            if (parameter != null && (Enum.Parse(current.GetType(), parameter.ToString()) is Enum param))
            {
                return Equals(current, param) ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}