using Microsoft.AspNetCore.Identity;

namespace Uber.Models.Domain
{
    public class UberUser: IdentityUser
    {
        public decimal? rating { get; set; }
        public int NumberOfReviews { get; set; } = 0;

    }
}
