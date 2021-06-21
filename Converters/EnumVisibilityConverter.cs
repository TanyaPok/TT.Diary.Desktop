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
            if (value is not Enum current)
            {
                return Visibility.Collapsed;
            }

            if (parameter == null) return Visibility.Collapsed;
            var parameters = parameter.ToString().Split('|');

            foreach (var param in parameters)
            {
                if (Enum.Parse(current.GetType(), param) is not Enum candidate)
                {
                    return Visibility.Collapsed;
                }

                if (Equals(current, candidate))
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}