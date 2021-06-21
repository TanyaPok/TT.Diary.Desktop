using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class CurrentDateBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var isDate = DateTime.TryParse(value.ToString(), out var date);

            if (parameter != null && parameter.ToString() == "range")
            {
                var rangeEnd = GetRangeEnd(date);
                return date <= DateTime.Now.Date && rangeEnd >= DateTime.Now.Date;
            }

            if (isDate)
            {
                return date.Date == DateTime.Now.Date;
            }

            return int.TryParse(value.ToString(), out var number)
                   && (number == DateTime.Now.Month || number == DateTime.Now.Year);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static DateTime GetRangeEnd(DateTime date)
        {
            var daysCount = DateTime.DaysInMonth(date.Year, date.Month);
            var finishDay = new DateTime(date.Year, date.Month, daysCount);
            var diff = 7 - (int) date.DayOfWeek;

            if (diff == 7)
            {
                return date;
            }

            var day = date.AddDays(diff);
            return (day > finishDay) ? finishDay : day;
        }
    }
}