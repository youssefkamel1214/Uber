using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Uber.Models.Domain
{
    public class Driver
    {
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicensePlate { get; set; }
        public string SSN { get; set; }
        public bool IsActive { get; set; } = false;
        public bool isAvailable { get; set; } = false;
        public decimal? Rating { get; set; }
        public virtual IdentityUser User { get; set; }

    }
}
