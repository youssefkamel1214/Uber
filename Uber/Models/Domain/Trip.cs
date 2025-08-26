using System.Text.Json.Serialization;
using Uber.Utils;

namespace Uber.Models.Domain
{
    public class Trip
    {
        public Guid TripId { get; set; } = Guid.NewGuid();
        public string? DriverId { get; set; }
        public string? PassengerId { get; set; }
        public Point Source { get; set; }
        public Point Destination { get; set; }
        public Decimal Distance { get; set; }
        public Decimal BasePrice { get; set; }
        public Decimal FinalPrice { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TripStatue Status { get; set; } 
        public DateTime RequestTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? BanTimeExires { get; set; }
        public virtual Passenger Passenger { get; set; }
        public virtual Driver Driver { get; set; }
    }
}
