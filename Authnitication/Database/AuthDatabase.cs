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
          
        }
    }
}
