using System;

namespace Projektledningsverktyg.Data.Entities
{
    public class DayView
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? ScheduleId { get; set; }
        public int? MealId { get; set; }
        public int? EventId { get; set; }
        public int? TaskId { get; set; }
        public int? HouseholdId { get; set; }

        public Schedule Schedule { get; set; }
        public Meal Meal { get; set; }
        public Event Event { get; set; }
        public Task Task { get; set; }
        public Household Household { get; set; }
    }
}
