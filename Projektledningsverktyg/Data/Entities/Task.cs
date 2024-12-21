using Projektledningsverktyg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Data.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        // Foreign keys
        public int MemberId { get; set; }
        public int ProjectId { get; set; }

        // Navigation properties
        public virtual Member AssignedTo { get; set; }
        public virtual Project Project { get; set; }
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}
