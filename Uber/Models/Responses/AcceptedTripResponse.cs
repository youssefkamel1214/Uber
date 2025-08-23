using Uber.Models.Domain;
using Uber.Utils;

namespace Uber.Models.Responses
{
    public class AcceptedTripResponse
    {
        public Guid TripId { get; set; } 
        public string? DriverId { get; set; }
        public string? PassengerId { get; set; }
        public Point source { get; set; }
        public Point destination { get; set; }
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
