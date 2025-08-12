using System.ComponentModel.DataAnnotations;

namespace Uber.Models.DTO.Reqeusts
{
    public class AcceptTenderDtoRequest
    {
        [Required]
        public Guid TenderId { get; set; }
    }
}
