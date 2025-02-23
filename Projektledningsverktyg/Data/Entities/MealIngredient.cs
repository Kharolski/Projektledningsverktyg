namespace Projektledningsverktyg.Data.Entities
{
    public class MealIngredient
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public decimal Amount { get; set; }
        public Units Unit { get; set; }
        public string Name { get; set; }
        public virtual Meal Meal { get; set; }
    }
}
