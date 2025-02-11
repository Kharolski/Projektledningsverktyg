﻿using Projektledningsverktyg.Data.Entities;
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

}
