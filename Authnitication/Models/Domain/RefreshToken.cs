using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authnitication.Models.Domain
{
    public class RefreshToken<TUser>
        where TUser : IdentityUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool isUsed { get; set; }
        public bool isRevoked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiresDate{ get; set; }
        [ForeignKey(nameof(UserId))]
        public TUser User { get; set; }
    }
}
