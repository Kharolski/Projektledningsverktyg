using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public int? MemberId { get; set; }
        public int? ProjectId { get; set; }

        // Navigation properties
        public virtual Member AssignedTo { get; set; }
        public virtual Project Project { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum TaskStatus
    {
        [Display(Name = "Ej påbörjad")]
        NotStarted,

        [Display(Name = "Pågående")]
        InProgress,

        [Display(Name = "Avslutad")]
        Completed,

        [Display(Name = "Pausad")]
        OnHold
    }

    public enum TaskPriority
    {
        [Display(Name = "Låg")]
        Low,

        [Display(Name = "Medel")]
        Medium,

        [Display(Name = "Hög")]
        High,

        [Display(Name = "Brådskande")]
        Urgent
    }
}
