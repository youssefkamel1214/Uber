using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories.interfaces
{
    public interface IUserIdentityAuthincation<TUSer>
        where TUSer : IdentityUser
    {
        public Task<IdentityResult> createUser(TUSer user,string password, string role);
        Task<TUSer> FindByEmailAsync(string email);
        public Task<TUSer> FindByIdAsync(string userId);
        public Task<List<string>> GetUserRolesAsync(TUSer user);
        public Task<TUSer?> signin(string useremail, string password);
    }
}
