using System.Collections.Generic;

namespace Projektledningsverktyg.Data.Entities
{
    public enum MealTypes
    {
        Frukost,
        Lunch,
        Middag,
        Efterrätt,
        Mellanmål
    }

    public enum MainIngredients
    {
        Kött,
        Fisk,
        Kyckling,
        Pasta,
        Vegetariskt,
        Vegansk
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MealTypes MealType { get; set; }
        public MainIngredients MainIngredient { get; set; }
        public int CookingTime { get; set; }
        public int Servings { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Instruction> Instructions { get; set; }
        public string ImagePath { get; set; }

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            Instructions = new List<Instruction>();
        }
    }
}
