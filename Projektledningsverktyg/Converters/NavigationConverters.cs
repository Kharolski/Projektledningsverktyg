using Projektledningsverktyg.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Projektledningsverktyg.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DayModel day)
            {
                int index = (int)day.Date.DayOfWeek - 1;
                return index < 0 ? 6 : index; // Handles Sunday (returns 6 instead of -1)
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsLastStepConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int stepNumber && values[1] is int totalSteps)
            {
                return stepNumber == totalSteps;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
