using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;
using System.Windows.Controls;
using System.Windows;

namespace Projektledningsverktyg.Widgets.WeekView
{
    /// <summary>
    /// En dragbar widget för generella uppgifter i veckovyn.
    /// Denna widget återanvänder GeneralTaskControl-komponenten.
    /// </summary>
    public class GeneralTaskWidget : DraggableWidget
    {
        private GeneralTaskControl _generalTaskControl;

        public GeneralTaskWidget()
        {
            // Sätt widget-identifierare - måste vara unik
            this.WidgetId = "WeekView_GeneralTaskWidget";

            // Skapa innehållet för widgeten
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Instansiera GeneralTaskControl (din befintliga kontroll)
            _generalTaskControl = new GeneralTaskControl();

            // Sätt det sammansatta UI:t som innehåll
            this.Content = _generalTaskControl;

            // Sätt lämpliga dimensioner för widgeten
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
