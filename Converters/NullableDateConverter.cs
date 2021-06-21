using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class NullableDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime nullableDate)
            {
                return value;
            }

            return nullableDate > DateTime.MinValue ? nullableDate.ToShortDateString() : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var str = value.ToString();
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return DateTime.TryParse(str, out var resultDateTime) ? resultDateTime : value;
        }
    }
}