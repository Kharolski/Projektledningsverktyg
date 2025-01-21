namespace Projektledningsverktyg.Data.Entities
{
    public enum Units
    {
        st,
        g,
        ml,
        msk,
        tsk,
        dl
    }

    public class Ingredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public decimal Amount { get; set; }
        public Units Unit { get; set; }
        public string Name { get; set; }
    }
}
