using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories.interfaces
{
    public interface IUserIdentityAuthincation
    {
        public Task<IdentityResult> createUser(string useremail, string password, string role, Func<string,Task<bool>> createprofile);
        public Task<IdentityUser> FindByIdAsync(string userId);
        public Task<List<string>> GetUserRolesAsync(IdentityUser user);
        public Task<IdentityUser?> signin(string useremail, string password);
    }
}
