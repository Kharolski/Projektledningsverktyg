using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace Projektledningsverktyg.Converters
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                // Uses current thread culture for localized date format
                return date.ToString("dddd d", Thread.CurrentThread.CurrentUICulture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CurrentDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCurrentDay && isCurrentDay)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#215b77")); // För att markera dagens datum
            }
            return new SolidColorBrush(Colors.Transparent); // Standard bakgrund för andra dagar
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
