using System.ComponentModel.DataAnnotations;

namespace Uber.Models.DTO.Reqeusts
{
    public class RequestTripDto
    {
        [Required]
        public string source { get; set; }
        [Required]
        public string destination { get; set; }
    }
}
