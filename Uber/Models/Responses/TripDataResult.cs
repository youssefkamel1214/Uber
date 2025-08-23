using System.Text.Json.Serialization;
using Uber.Models.Domain;
using Uber.Utils;

namespace Uber.Models.Responses
{
    public class TripDataResult
    {
        public string tripId { get; set; }
        public string PassengerName { get; set; }
        public decimal? PassengerRating { get; set; }
        public Point source { get; set; }
        public Point destination { get; set; }
        public Decimal Distance { get; set; }
        public Decimal BasePrice { get; set; }
        public DateTime RequestTime { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TripStatue Status { get; set; }
    }
}
