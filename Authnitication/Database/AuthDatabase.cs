using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Database
{
    public class AuthDatabase: IdentityDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AuthDatabase(DbContextOptions<AuthDatabase> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var readerRoleId = "59ed7de2-10a3-417d-9255-e83e16363d16";
            var writerRoleId = "3848fb35-56df-4772-91ef-d5c16c005d79";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Driver",
                    NormalizedName = "Driver".ToUpper()
                },
                new IdentityRole
                {
                    Id=writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
