using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;
using System.Windows.Controls;
using System.Windows;

namespace Projektledningsverktyg.Widgets.WeekView
{
    /// <summary>
    /// En dragbar widget för händelser/events i veckovyn.
    /// Denna widget återanvänder EventsControl-komponenten.
    /// </summary>
    public class EventWidget : DraggableWidget
    {
        private EventsControl _eventsControl;

        public EventWidget()
        {
            // Sätt widget-identifierare - måste vara unik
            this.WidgetId = "WeekView_EventWidget";

            // Skapa innehållet för widgeten
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Instansiera EventsControl (din befintliga kontroll)
            _eventsControl = new EventsControl();

            // Sätt det sammansatta UI:t som innehåll
            this.Content = _eventsControl;

            // Sätt lämpliga dimensioner för widgeten
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
