using System;
using System.Globalization;
using System.Windows.Data;

namespace Projektledningsverktyg.Converters
{
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
}
