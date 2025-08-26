using Authnitication.Database;
using Authnitication.Models.Domain;
using Authnitication.Reposotories.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories
{
    public class RefershTokenRepository<TDBataBase,TUser> : IRefershTokenReposotiry<TDBataBase,TUser>
        where TUser : IdentityUser
        where TDBataBase : DbContext, IRefershTokenDataBase<TUser>

    {
        private readonly TDBataBase _authDatabase;

        public RefershTokenRepository(TDBataBase authDatabase)
        {
            _authDatabase = authDatabase;
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken<TUser> refreshToken)
        {
            await _authDatabase.RefreshTokens.AddAsync(refreshToken);
            await _authDatabase.SaveChangesAsync();
            return true;
        }

        public async Task<RefreshToken<TUser>?> GetRefreshTokenByIdAsync(string RefreshToken)
        {
            var storedToken = await _authDatabase.RefreshTokens
                                       .FirstOrDefaultAsync(x => x.Token == RefreshToken);
            return storedToken; // Returns null if not found
        }

        public async Task MakeOldRefreshTokenRevekod(string UserId, string IpAdress)
        {
            await _authDatabase.RefreshTokens.Where(rt => rt.UserId == UserId && rt.IpAddress == IpAdress && !rt.isRevoked&&!rt.isUsed)
                 .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.isRevoked, true));
        }

        public async Task<bool> UpdateRefreshTokenAsync(RefreshToken<TUser> refreshToken)
        {
            var storedToken = await _authDatabase.RefreshTokens
                           .FirstOrDefaultAsync(x => x.Token == refreshToken.Token);
            if (storedToken == null)
                {
                return false; // Token not found
            }
            storedToken.isRevoked = refreshToken.isRevoked;
            storedToken.isUsed= refreshToken.isUsed;
            _authDatabase.RefreshTokens.Update(storedToken);
            await _authDatabase.SaveChangesAsync();
            return true; // Update successful
        }
    }
}
