using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Uber.Models.Domain
{
    public class Passenger
    {
        public string PassngerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public  decimal? rating { get; set; }
        public int CancellationCount { get; set; }=0;
        public virtual IdentityUser User { get; set; }


    }
}
