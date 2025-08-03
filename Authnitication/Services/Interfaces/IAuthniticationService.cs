using Authnitication.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Services.Interfaces
{
    public interface IAuthniticationService
    {
        public Task<AuthResult> CreateUserAsync(string userEmail, string password, string role,Func<string,Task< bool>> createprofile);
        public Task<AuthResult> signInUser(string userEmail, string password);
        public Task<AuthResult> getrefeshedtoken(string token, string refersh);
    }
}
