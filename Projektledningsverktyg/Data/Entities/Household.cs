namespace Projektledningsverktyg.Data.Entities
{
    public class Household
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? MemberId { get; set; }  // Optional link to member
        public Member Member { get; set; }   // Navigation property
    }
}
