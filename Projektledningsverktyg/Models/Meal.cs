using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;

namespace Projektledningsverktyg.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MealType Type { get; set; }  
        public DateTime Date { get; set; }

        // Nutritional Information
        public int Calories { get; set; }
        public float Protein { get; set; }
        public float Carbohydrates { get; set; }
        public float Fat { get; set; }
        public float Fiber { get; set; }

        // Recipe Details
        public List<string> Ingredients { get; set; } = new List<string>();
        public string Instructions { get; set; }
        public int PreparationTime { get; set; }
        public int ServingSize { get; set; }

        // Additional Info
        public List<string> DietaryTags { get; set; } = new List<string>();
        public string Notes { get; set; }
    }

    
}
