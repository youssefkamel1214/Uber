using Authnitication.Database;
using Authnitication.Models.Domain;
using Authnitication.Reposotories.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Reposotories
{
    public class UserIdentityAuthincation : IUserIdentityAuthincation
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDatabase _authDatabase;

        public UserIdentityAuthincation(UserManager<IdentityUser> userManager, AuthDatabase authDatabase)
        {
            _userManager = userManager;
            _authDatabase = authDatabase;
        }

        public async Task<IdentityResult> createUser(string useremail, string password, string role, Func<string, Task<bool>> createprofile)
        {
            var user = new IdentityUser { UserName = useremail, Email = useremail };
            var existingUser = await _userManager.FindByEmailAsync(useremail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists with this email.");
            }
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
              throw new InvalidOperationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            await  _userManager.AddToRoleAsync(user, role);
            bool success = false;
            string error="";
            try 
            { 
                 success= await createprofile(user.Id);
            }catch (Exception ex)
            {
                success = false;
                error = $"{ex.Message}";
            }
            if (!success)
            {
                await _userManager.DeleteAsync(user);
                throw new InvalidOperationException($"{error}");
            }
            return result;
        }

        public async Task<IdentityUser> FindByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any()) 
            {
                throw new InvalidOperationException("User has no roles assigned.");
            }
            return roles.ToList();
        }

        public async Task<IdentityUser?> signin(string useremail, string password)
        {
            IdentityUser? user = await _userManager.FindByEmailAsync(useremail);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
        }
    }
}
