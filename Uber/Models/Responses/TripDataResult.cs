using Uber.Utils;

namespace Uber.Models.Responses
{
    public class TripDataResult
    {
        public string PassengerName { get; set; }
        public decimal? PassengerRating { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public Decimal Distance { get; set; }
        public Decimal BasePrice { get; set; }
        public DateTime RequestTime { get; set; }
        public TripStatue Status { get; set; }
    }
}
