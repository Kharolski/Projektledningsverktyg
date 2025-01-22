namespace Projektledningsverktyg.Data.Entities
{
    public class Instruction
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int PrepTime { get; set; }
    }
}
