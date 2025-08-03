using Authnitication.Database;
using Authnitication.Models.Domain;
using Authnitication.Reposotories.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories
{
    public class RefershTokenRepository : IRefershTokenReposotiry
      
    {
        private readonly AuthDatabase _authDatabase;

        public RefershTokenRepository(AuthDatabase authDatabase)
        {
            _authDatabase = authDatabase;
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _authDatabase.RefreshTokens.AddAsync(refreshToken);
            await _authDatabase.SaveChangesAsync();
            return true;
        }

        public async Task<RefreshToken?> GetRefreshTokenByIdAsync(string RefreshToken)
        {
            var storedToken = await _authDatabase.RefreshTokens
                                       .FirstOrDefaultAsync(x => x.Token == RefreshToken);
            return storedToken; // Returns null if not found
        }

        public async Task<bool> UpdateRefreshTokenAsync(RefreshToken refreshToken)
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
