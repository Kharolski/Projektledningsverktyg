using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projektledningsverktyg.Data.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public EventType Type { get; set; }


        // Foreign keys
        public int? ProjectId { get; set; }


        // Navigation properties
        public virtual Member Creator { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Member> Participants { get; set; }

        public Event()
        {
            Participants = new HashSet<Member>();
        }
    }

    public enum EventType
    {
        [Display(Name = "Födelsedagar")]
        Birthday,    // Födelsedagar 🎂
        [Display(Name = "Utflykter/Resor")]
        Travel,      // Utflykter/Resor 🚗
        [Display(Name = "Möten")]
        Meeting,     // Möten 👥
        [Display(Name = "Övrigt")]
        Other        // Övrigt ✨
    }
}
