using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;
using System.Windows.Controls;
using System.Windows;

namespace Projektledningsverktyg.Widgets.WeekView
{
    /// <summary>
    /// En dragbar widget för hushållshantering på veckovyn.
    /// Denna widget återanvänder HouseholdControl-komponenten.
    /// </summary>
    public class HouseholdWidget : DraggableWidget
    {
        private HouseholdControl _householdControl;

        public HouseholdWidget()
        {
            // Sätt widget-identifierare - måste vara unik
            this.WidgetId = "WeekView_HouseholdWidget";

            // Skapa innehållet för widgeten
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Instansiera HouseholdControl (din befintliga kontroll)
            _householdControl = new HouseholdControl();

            // Sätt det sammansatta UI:t som innehåll
            this.Content = _householdControl;

            // Sätt lämpliga dimensioner för widgeten
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
