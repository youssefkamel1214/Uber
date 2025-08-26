using Authnitication.Database;
using Authnitication.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Services.Interfaces
{
    public interface IAuthniticationService<TUser, TDataBase>
        where TUser : IdentityUser
        where TDataBase : DbContext, IRefershTokenDataBase<TUser>
    {
        public Task<AuthResult> CreateUserAsync(TUser userdata, string password, string role,string IpAdress);
        public Task<AuthResult> signInUser(string userEmail, string password, string IpAdress);
        public Task<AuthResult> getrefeshedtoken(string token, string refersh, string IpAdress);
        protected Task<AuthResult> GenerateJwtToken(IdentityUser user, List<string> roles, string IpAdress);

    }
}
