using Microsoft.AspNetCore.Identity;


namespace Uber.Models.Domain
{
    public class Cancellation
    {
        public Guid CancellationId { get; set; } = Guid.NewGuid();
        public Guid TripId { get; set; }
        public decimal Fee { get; set; }
        public string CancelledBy { get; set; } // Enum to indicate who cancelled the trip
        public String? Reason { get; set; } // Reason for cancellation
        public bool IsRefunded { get; set; } = false; // Indicates if money was refunded to user because it has sautible reason
        public DateTime CancellationTime { get; set; } // Time of cancellation
        public virtual Trip Trip { get; set; }
        public virtual IdentityUser User { get; set; }

    }
}
