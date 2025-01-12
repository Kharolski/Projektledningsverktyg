using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projektledningsverktyg.Data.Entities
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
        public string Instructions { get; set; }
        public int PreparationTime { get; set; }
        public int ServingSize { get; set; }
        public string Notes { get; set; }

        // Navigation properties for Ingredients and DietaryTags
        public virtual ICollection<MealIngredient> Ingredients { get; set; } = new List<MealIngredient>();
        public virtual ICollection<MealDietaryTag> DietaryTags { get; set; } = new List<MealDietaryTag>();
    }

    public enum MealType
    {
        [Display(Name = "Frukost")]
        Breakfast,

        [Display(Name = "Lunch")]
        Lunch,

        [Display(Name = "Middag")]
        Dinner,

        [Display(Name = "Mellanmål")]
        Snack
    }
}
