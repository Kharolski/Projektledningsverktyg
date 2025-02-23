using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projektledningsverktyg.Data.Entities
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MealType Type { get; set; }
        public MainIngredients MainIngredient { get; set; }
        public int CookingTime { get; set; }
        public int Servings { get; set; }
        public DateTime Date { get; set; }
        public string ImagePath { get; set; }
        public int RecipeId { get; set; }  // Reference to original recipe

        // Navigation properties
        public virtual ICollection<MealIngredient> Ingredients { get; set; }
        public virtual ICollection<MealInstruction> Instructions { get; set; }

        public Meal()
        {
            Ingredients = new List<MealIngredient>();
            Instructions = new List<MealInstruction>();
        }
    }

    public enum MealType
    {
        [Display(Name = "Frukost")]
        Frukost,

        [Display(Name = "Lunch")]
        Lunch,

        [Display(Name = "Middag")]
        Middag,

        [Display(Name = "Efterrätt")]
        Efterrätt,

        [Display(Name = "Mellanmål")]
        Mellanmål
    }
}
