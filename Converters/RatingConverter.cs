using System;
using System.Globalization;
using System.Windows.Data;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.Converters
{
    public class RatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            return (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Rating.Empty;
            }

            return (Rating) value;
        }
    }
}