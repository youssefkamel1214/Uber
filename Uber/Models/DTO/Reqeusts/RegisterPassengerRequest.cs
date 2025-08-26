using System.ComponentModel.DataAnnotations;

namespace Uber.Models.DTO.Reqeusts
{
    public class RegisterPassengerRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 12 digits.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be numeric and exactly 12 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
