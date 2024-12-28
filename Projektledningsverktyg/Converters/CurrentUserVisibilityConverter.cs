// Required namespaces for converter functionality
using System;
using System.Windows;           // For Visibility enum
using System.Windows.Data;      // For IValueConverter interface
using System.Globalization;     // For CultureInfo

namespace Projektledningsverktyg.Converters
{
    // Converter class that implements IValueConverter to transform values in XAML bindings
    public class CurrentUserVisibilityConverter : IValueConverter
    {
        // Convert method: transforms the source value to the target type
        // Used when data flows from source to target (Model to View)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Cast the incoming value to int (member ID)
            int memberId = (int)value;
            // Get the current logged-in user's ID
            int currentUserId = App.CurrentUser.Id;
            // Return Collapsed if IDs match (hide button), Visible if they don't
            return memberId == currentUserId ? Visibility.Collapsed : Visibility.Visible;
        }

        // ConvertBack method: transforms the target value back to the source type
        // Not needed for one-way bindings, so we throw NotImplementedException
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
