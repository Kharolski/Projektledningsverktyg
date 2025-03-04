using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;
using System.Windows.Controls;
using System.Windows;

namespace Projektledningsverktyg.Widgets.WeekView
{
    /// <summary>
    /// En dragbar widget för schemahantering på veckovyn.
    /// Denna widget återanvänder ScheduleControl-komponenten.
    /// </summary>
    public class ScheduleWidget : DraggableWidget
    {
        private ScheduleControl _scheduleControl;

        public ScheduleWidget()
        {
            // Sätt widget-identifierare - måste vara unik
            this.WidgetId = "WeekView_ScheduleWidget";

            // Skapa innehållet för widgeten
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Instansiera ScheduleControl (din befintliga kontroll)
            _scheduleControl = new ScheduleControl();

            // Sätt det sammansatta UI:t som innehåll
            this.Content = _scheduleControl;

            // Sätt lämpliga dimensioner för widgeten
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
