using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }

        // Navigation properties
        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual Member ProjectManager { get; set; }
        public int ProjectManagerId { get; set; }
    }

    public enum ProjectStatus
    {
        Planning,
        Active,
        OnHold,
        Completed,
        Cancelled
    }
}
