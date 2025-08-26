using Authnitication.Database;
using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories.interfaces
{
    public interface IRefershTokenReposotiry<TDBataBase,TUser>
        where TUser : IdentityUser
        where TDBataBase : DbContext, IRefershTokenDataBase<TUser>
    {
        public Task<bool> AddRefreshTokenAsync(RefreshToken<TUser> refreshToken);
        public Task<bool> UpdateRefreshTokenAsync(RefreshToken<TUser> refreshToken);
        public Task<RefreshToken<TUser>?> GetRefreshTokenByIdAsync(string RefreshToken);
        public Task MakeOldRefreshTokenRevekod(string UserId, string IpAdress);
    }
}
