using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Uber.Utils;

namespace Uber.Models.Domain
{
    public class Review
    {
        public Guid ReviewId { get; set; }=Guid.NewGuid();
        public Guid TripId { get; set; }
        public string? ReviewerId { get; set; } // User who gives the review
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ReviewType type { get; set; } 
        public int Rating { get; set; } // Rating out of 5
        public string? Comment { get; set; } // Optional comment

    }
}
