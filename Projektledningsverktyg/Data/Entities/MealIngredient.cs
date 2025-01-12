using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Data.Entities
{
    public class MealIngredient
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public virtual Meal Meal { get; set; }
    }
}
