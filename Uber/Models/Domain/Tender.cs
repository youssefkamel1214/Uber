namespace Uber.Models.Domain
{
    public class Tender
    {
        public Guid TripId { get; set; }
        public string DriverId { get; set; }
        public DateTime TenderTime { get; set; }
        public decimal OfferedPrice { get; set; }
    }
}
