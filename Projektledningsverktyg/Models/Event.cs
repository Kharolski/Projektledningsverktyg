using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
    }

    public enum EventType
    {
        Meeting,
        Birthday,
        Task,
        Other
    }
}
