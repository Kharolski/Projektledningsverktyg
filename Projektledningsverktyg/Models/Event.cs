using System;

namespace Projektledningsverktyg.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
    }

    public enum EventType
    {
        Birthday,    
        Travel,      
        Meeting,     
        Other        
    }
}
