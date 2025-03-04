using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projektledningsverktyg.Views.Settings
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Denna funktion är inte tillgänglig i demo-versionen.",
                           "Demo-begränsning", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dataexport är inte implementerad i demo-versionen.",
                           "Demo-begränsning", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dataimport är inte implementerad i demo-versionen.",
                           "Demo-begränsning", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            // Simulera rensning av cache genom att visa en laddningsindikator
            Mouse.OverrideCursor = Cursors.Wait;

            // Simulera fördröjning
            System.Threading.Thread.Sleep(500);

            Mouse.OverrideCursor = null;
            MessageBox.Show("Cache har rensats.", "Åtgärd slutförd",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
