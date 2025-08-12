using Uber.Utils;

namespace Uber.Models.Domain
{
    public class Tender
    {
        public Guid TenderId { get; set; }= Guid.NewGuid();
        public Guid TripId { get; set; }
        public string DriverId { get; set; }
        public DateTime TenderTime { get; set; }
        public decimal OfferedPrice { get; set; }
        public TenderStatue staute { get; set; } = TenderStatue.WaitingForPassenger;
    }
}
