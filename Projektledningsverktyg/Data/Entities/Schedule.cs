using System;
using System.ComponentModel.DataAnnotations;

namespace Projektledningsverktyg.Data.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }  // Nullable for flexible timing
        public DateTime? EndTime { get; set; }    // Nullable for flexible timing
        public ScheduleType Type { get; set; }    // Enum for Job/School or Deviation
    }

    public enum ScheduleType
    {
        [Display(Name = "Jobb - Skola")]
        WorkSchool,

        [Display(Name = "Avvikande")]
        Deviation
    }
}
