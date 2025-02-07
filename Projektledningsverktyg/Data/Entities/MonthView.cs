using System;

namespace Projektledningsverktyg.Data.Entities
{
    public class MonthView
    {
        public int Id { get; set; }
        public DateTime Month { get; set; }
        public int? ScheduleId { get; set; }
        public int? EventId { get; set; }
        public Schedule Schedule { get; set; }
        public Event Event { get; set; }
    }

}
