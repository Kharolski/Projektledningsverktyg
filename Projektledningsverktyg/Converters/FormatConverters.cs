using Projektledningsverktyg.Data.Entities;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace Projektledningsverktyg.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                return path.Replace("\\", "/");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class EmptyToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? "Spara" : "Ändra";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

    public class EventTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Models.EventType eventType)
            {
                switch (eventType)
                {
                    case Models.EventType.Birthday:
                        return "🎂";
                    case Models.EventType.Travel:
                        return "🚗";
                    case Models.EventType.Meeting:
                        return "👥";
                    case Models.EventType.Other:
                        return "✨";
                    default:
                        return "📅";
                }
            }
            return "📅";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
