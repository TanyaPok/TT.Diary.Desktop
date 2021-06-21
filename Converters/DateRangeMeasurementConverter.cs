using System;
using System.Globalization;
using System.Windows.Data;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.Converters
{
    public class DateRangeMeasurementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var current = Enum.Parse(typeof(Repeat), value.ToString());
            switch (current)
            {
                case Repeat.Daily: return "day(s)";
                case Repeat.Weekly: return "week(s)";
                case Repeat.Monthly: return "month(s)";
                case Repeat.Yearly: return "year(s)";
                default: return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}