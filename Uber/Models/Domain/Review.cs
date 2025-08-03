namespace Uber.Models.Domain
{
    public class Review
    {
        public Guid ReviewId { get; set; }=Guid.NewGuid();
        public Guid TripId { get; set; }
        public int Rating { get; set; } // Rating out of 5
        public string Comment { get; set; } // Optional comment
    }
}
