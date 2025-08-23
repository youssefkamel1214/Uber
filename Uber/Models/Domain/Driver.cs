using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Uber.Models.Domain
{
    public class Driver:UberUser
    {
        public string LicensePlate { get; set; }
        public string SSN { get; set; }
        public bool IsActive { get; set; } = false;
        public bool isAvailable { get; set; } = false;
    }
}
