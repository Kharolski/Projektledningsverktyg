using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Data.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public EventType Type { get; set; }


        // Foreign keys
        public int CreatorId { get; set; }
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
        Meeting,
        Deadline,
        Milestone,
        Other
    }
}
