using System.ComponentModel.DataAnnotations;

namespace Uber.Models.DTO.Reqeusts
{
    public class AddTenderRequestDto
    {
        [Required]
        public Guid TripId { get; set; }
        [Required]
        public decimal OfferedPrice { get; set; }
    }
}
