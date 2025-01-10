using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;

namespace Projektledningsverktyg.Models
{
    // This data model - represents how task data is stored
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public int MemberId { get; set; }
        public int ProjectId { get; set; }

    }

}
