using Authnitication.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories.interfaces
{
    public interface IRefershTokenReposotiry
    {
        public Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken);
        public Task<bool> UpdateRefreshTokenAsync(RefreshToken refreshToken);
        public Task<RefreshToken?> GetRefreshTokenByIdAsync(string RefreshToken);
    }
}
