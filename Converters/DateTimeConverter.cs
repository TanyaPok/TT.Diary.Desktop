using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        private DateTime _dateTime;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _dateTime = (DateTime)value;

            if (parameter.ToString() == "h")
            {
                return _dateTime.Hour;
            }

            if (parameter.ToString() == "m")
            {
                return _dateTime.Minute;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (int)value;

            if (parameter.ToString() == "h")
            {
                return _dateTime.AddHours(-_dateTime.Hour).AddHours(time);
            }

            if (parameter.ToString() == "m")
            {
                return _dateTime.AddMinutes(-_dateTime.Minute).AddMinutes(time);
            }

            return _dateTime;
        }
    }
}
