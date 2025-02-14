using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Projektledningsverktyg.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority priority)
            {
                switch (priority)
                {
                    case TaskPriority.Urgent:
                        return new SolidColorBrush(Colors.Red);
                    case TaskPriority.High:
                        return new SolidColorBrush(Color.FromRgb(255, 69, 0));  // OrangeRed
                    case TaskPriority.Medium:
                        return new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Orange
                    case TaskPriority.Low:
                        return new SolidColorBrush(Color.FromRgb(50, 205, 50)); // LimeGreen
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EventTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EventType eventType)  // Specify the full type
            {
                switch (eventType)
                {
                    case EventType.Birthday:
                        return new SolidColorBrush(Color.FromRgb(255, 20, 147));
                    case EventType.Travel:
                        return new SolidColorBrush(Color.FromRgb(65, 105, 225));
                    case EventType.Meeting:
                        return new SolidColorBrush(Color.FromRgb(50, 205, 50));
                    case EventType.Other:
                        return new SolidColorBrush(Color.FromRgb(255, 140, 0));
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
