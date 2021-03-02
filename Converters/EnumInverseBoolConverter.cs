using System;
using System.Globalization;
using System.Windows.Data;

namespace TT.Diary.Desktop.Converters
{
    public class EnumInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as Enum;

            if (current == null)
            {
                return false;
            }

            var param = Enum.Parse(current.GetType(), parameter.ToString()) as Enum;

            if (param == null)
            {
                return false;
            }

            return !Enum.Equals(current, param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
