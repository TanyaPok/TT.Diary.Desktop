using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class NullableDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullableDate = value as Nullable<DateTime>;

            if (nullableDate == null)
            {
                return value;
            }

            if (nullableDate.HasValue && nullableDate.Value > DateTime.MinValue)
            {
                return nullableDate.Value.ToShortDateString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();

            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            if (DateTime.TryParse(str, out DateTime resultDateTime))
            { 
                return resultDateTime; 
            }

            return value;
        }
    }
}
