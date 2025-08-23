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
    public class UserIdentityAuthincation <TUser> : IUserIdentityAuthincation<TUser>
        where TUser : IdentityUser
    {
        private readonly UserManager<TUser> _userManager;

        public UserIdentityAuthincation(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> createUser(TUser user,string password,string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
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
            return result;
        }

        public async Task<TUser> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }

        public async Task<TUser> FindByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(TUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any()) 
            {
                throw new InvalidOperationException("User has no roles assigned.");
            }
            return roles.ToList();
        }

        public async Task<TUser?> signin(string useremail, string password)
        {
            TUser? user = await _userManager.FindByEmailAsync(useremail);
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
