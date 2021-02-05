using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class EnumVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as Enum;

            if (current == null)
            {
                return Visibility.Collapsed;
            }

            var param = Enum.Parse(current.GetType(), parameter.ToString()) as Enum;

            if (param == null)
            {
                return Visibility.Collapsed;
            }

            return Enum.Equals(current, param) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}