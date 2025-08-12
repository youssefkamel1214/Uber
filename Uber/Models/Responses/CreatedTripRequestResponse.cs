using Uber.Utils;

namespace Uber.Models.Responses
{
    public class CreatedTripRequestResponse: TripServiceResponse
    {
        public Guid TripId { get; set; } = Guid.NewGuid();
        public string PassengerId { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public Decimal Distance { get; set; }
        public Decimal BasePrice { get; set; }
        public string Status { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
