using System.ComponentModel.DataAnnotations;

namespace Uber.Models.DTO.Reqeusts
{
    public class ReviewRequestDto
    {
        [Required]
        public Guid TripId { get; set; }
        [Required]
        public int Rating { get; set; } // Rating out of 5
        public string? Comment { get; set; } // Optional comment
    }
}
