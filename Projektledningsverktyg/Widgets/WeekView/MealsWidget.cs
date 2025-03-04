using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;
using System.Windows.Controls;
using System.Windows;

namespace Projektledningsverktyg.Widgets.WeekView
{
    /// <summary>
    /// En dragbar widget för måltidshantering på veckovyn.
    /// Denna widget återanvänder MealsControl-komponenten.
    /// </summary>
    public class MealsWidget : DraggableWidget
    {
        private MealsControl _mealsControl;

        public MealsWidget()
        {
            // Sätt widget-identifierare - måste vara unik
            this.WidgetId = "WeekView_MealsWidget";

            // Skapa innehållet för widgeten
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Instansiera MealsControl (din befintliga kontroll)
            _mealsControl = new MealsControl();

            // Sätt det sammansatta UI:t som innehåll
            this.Content = _mealsControl;

            // Sätt lämpliga dimensioner för widgeten
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
