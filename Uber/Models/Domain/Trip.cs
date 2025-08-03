using Uber.Utils;

namespace Uber.Models.Domain
{
    public class Trip
    {
        public Guid TripId { get; set; } = Guid.NewGuid();
        public string DriverId { get; set; }
        public string PassengerId { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public Decimal Distance { get; set; }
        public Decimal BasePrice { get; set; }
        public Decimal FinalPrice { get; set; }
        public TripStatue Status { get; set; } 
        public DateTime RequestTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal CancellationFee { get; set; }
    }
}
