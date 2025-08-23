using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Uber.Models.Domain
{
    public class Passenger:UberUser
    {
        public Point? Home { get; set; }
        public Point? Work { get; set; }
        public string?paymentMethod { get; set; }

    }
}
