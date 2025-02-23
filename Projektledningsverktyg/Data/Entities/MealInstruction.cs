namespace Projektledningsverktyg.Data.Entities
{
    public class MealInstruction
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int PrepTime { get; set; }
        public virtual Meal Meal { get; set; }
    }
}
