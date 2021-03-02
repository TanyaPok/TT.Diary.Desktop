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

            var parameters = parameter.ToString().Split('|');

            foreach (var param in parameters)
            {
                var candidate = Enum.Parse(current.GetType(), param.ToString()) as Enum;

                if (candidate == null)
                {
                    return Visibility.Collapsed;
                }

                if (Enum.Equals(current, candidate))
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