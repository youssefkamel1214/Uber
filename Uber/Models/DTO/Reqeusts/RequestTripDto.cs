using System.ComponentModel.DataAnnotations;
using Uber.Models.Domain;

namespace Uber.Models.DTO.Reqeusts
{
    public class RequestTripDto
    {
        [Required]
        public Point source { get; set; }
        [Required]
        public Point destination { get; set; }
    }
}
