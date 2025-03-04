using System;

namespace Projektledningsverktyg.Data.Entities
{
    public class WidgetPosition
    {
        public int Id { get; set; }

        // Främmande nyckel till Member
        public int MemberId { get; set; }

        // Widget identifierare (t.ex. "MealsWidget", "TasksWidget")
        public string WidgetId { get; set; }

        // Position i grid-layouten
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        // Om widgeten är synlig
        public bool IsVisible { get; set; } = true;

        // Tidsstämpel för senaste uppdatering
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigationsegenskap till Member
        public virtual Member Member { get; set; }
    }
}
