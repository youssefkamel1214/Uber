using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Database
{
    /*
     * Here we define the interface for the RefreshToken database context.
     * Database Contexts should implement this interface to ensure that RefreshTokens Used in the application
     */
    public interface IRefershTokenDataBase<TUser> where TUser: IdentityUser
    {
        public DbSet<RefreshToken<TUser>> RefreshTokens { get; set; }
    }
}
