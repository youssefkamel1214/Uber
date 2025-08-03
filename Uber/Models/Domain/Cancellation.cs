using Uber.Utils;

namespace Uber.Models.Domain
{
    public class Cancellation
    {
        public Guid CancellationId { get; set; } = Guid.NewGuid();
        public Guid TripId { get; set; }
        public CancelledBy cancelledBy { get; set; } // Enum to indicate who cancelled the trip
        public String Reason { get; set; } // Reason for cancellation
        public bool IsRefunded { get; set; } // Indicates if money was refunded to user because it has sautible reason
        public DateTime CancellationTime { get; set; } // Time of cancellation
    }
}
