using System.Collections.Generic;

namespace Projektledningsverktyg.Data.Entities
{
    public class Household
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Navigation property for many-to-many
        public virtual ICollection<HouseholdAssignment> Assignments { get; set; } = new List<HouseholdAssignment>();
    }
}
