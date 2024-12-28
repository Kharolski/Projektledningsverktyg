using System;
using System.Collections.Generic;

namespace Projektledningsverktyg.Data.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public string Role { get; set; }
        public DateTime BirthDate { get; set; }
        public string ProfileImagePath { get; set; }
        public bool IsAdmin { get; set; }

        // Navigation properties
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Event> CreatedEvents { get; set; }
        public virtual ICollection<Event> ParticipatingEvents { get; set; }

        public Member()
        {
            CreatedEvents = new HashSet<Event>();
            ParticipatingEvents = new HashSet<Event>();
        }
    }
}
