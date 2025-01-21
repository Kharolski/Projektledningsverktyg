using Projektledningsverktyg.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Projektledningsverktyg.Converters
{
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
