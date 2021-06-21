using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        private readonly string _h = "h";
        private readonly string _m = "m";
        private DateTime _dateTime;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return 0;
            }

            _dateTime = (DateTime) value;

            if (parameter.ToString() == _h)
            {
                return _dateTime.Hour;
            }

            return parameter.ToString() == _m ? _dateTime.Minute : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return _dateTime;
            }

            var time = (int) value;

            if (parameter.ToString() == _h)
            {
                return _dateTime.AddHours(-_dateTime.Hour).AddHours(time);
            }

            return parameter.ToString() == _m ? _dateTime.AddMinutes(-_dateTime.Minute).AddMinutes(time) : _dateTime;
        }
    }
}