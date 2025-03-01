using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Helpers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
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
            // Check if this is for the IsSelected property
            if (parameter != null && parameter.ToString() == "Selected")
            {
                // If this is a selected day check
                if (value is bool isSelected && isSelected)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D47A1")); // Darker blue for selected day
                }
                return new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                // This is the original current day check
                if (value is bool isCurrentDay && isCurrentDay)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#215b77")); // Current day highlight
                }
                return new SolidColorBrush(Colors.Transparent); // Standard background for other days
            }
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
            if (value is EventType eventType)
            {
                switch (eventType)
                {
                    case EventType.Birthday:
                        return "🎂";
                    case EventType.Travel:
                        return "🚗";
                    case EventType.Meeting:
                        return "👥";
                    case EventType.Other:
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

    public class TimeDisplayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var startTime = values[0] as DateTime?;
            var endTime = values[1] as DateTime?;

            if (startTime.HasValue && endTime.HasValue)
            {
                return $"{startTime.Value:HH:mm} - {endTime.Value:HH:mm}";
            }

            if (startTime.HasValue && !endTime.HasValue)
            {
                return $"{startTime.Value:HH:mm} - " + "...";
            }

            return "Under dagen";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PriorityDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority priority)
            {
                return EnumDisplayHelper.GetDisplayName(priority);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string displayName)
            {
                foreach (TaskPriority priority in Enum.GetValues(typeof(TaskPriority)))
                {
                    if (EnumDisplayHelper.GetDisplayName(priority) == displayName)
                        return priority;
                }
            }
            return TaskPriority.Low;
        }
    }

    public class ScheduleDisplayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var title = values[0] as string;
            var startTime = values[1] as DateTime?;
            var endTime = values[2] as DateTime?;

            var timeDisplay = GetTimeDisplay(startTime, endTime);
            return $"{title} ({timeDisplay})";
        }

        private string GetTimeDisplay(DateTime? startTime, DateTime? endTime)
        {
            if (startTime.HasValue && endTime.HasValue)
                return $"{startTime.Value:HH:mm} - {endTime.Value:HH:mm}";

            if (startTime.HasValue)
                return $"{startTime.Value:HH:mm}";

            return "Under dagen";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
