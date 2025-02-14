using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Data.Entities
{
    public class HouseholdAssignment
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public int MemberId { get; set; }
        public string AssignedDays { get; set; } = "[]";

        // Navigation properties
        public virtual Household Household { get; set; }
        public virtual Member Member { get; set; }
    }
}
