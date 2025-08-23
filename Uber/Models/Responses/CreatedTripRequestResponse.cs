using Uber.Models.Domain;
using Uber.Utils;

namespace Uber.Models.Responses
{
    public class CreatedTripRequestResponse
    {
        public Guid TripId { get; set; } = Guid.NewGuid();
        public string PassengerId { get; set; }
        public Point source { get; set; }
        public Point destination { get; set; }
        public Decimal Distance { get; set; }
        public Decimal BasePrice { get; set; }
        public string Status { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
