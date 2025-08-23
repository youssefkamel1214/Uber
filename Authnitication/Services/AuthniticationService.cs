using Authnitication.Database;
using Authnitication.Models.Domain;
using Authnitication.Models.Responses;
using Authnitication.Reposotories.interfaces;
using Authnitication.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authnitication.Services
{
    public class AuthniticationService<TUser,TDataBase> : IAuthniticationService<TUser,TDataBase>
        where TUser : IdentityUser
        where TDataBase : DbContext, IRefershTokenDataBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRefershTokenReposotiry<TDataBase> _refershtokenReposiroty;
        private readonly IUserIdentityAuthincation<TUser> _userIdentityAuthincation;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthniticationService(IConfiguration configuration, IRefershTokenReposotiry<TDataBase> refershtokenReposiroty,
            IUserIdentityAuthincation<TUser> userIdentityAuthincation, TokenValidationParameters tokenValidationParameters)
        {
            _configuration = configuration;
            _refershtokenReposiroty = refershtokenReposiroty;
            _userIdentityAuthincation = userIdentityAuthincation;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthResult> CreateUserAsync(TUser userdata, string password, string role)
        {
            try
            {

               
                var identityresult = await _userIdentityAuthincation.createUser(userdata, password, role);
                if (!identityresult.Succeeded)
                {
                    return new AuthResult()
                    {
                        Error = identityresult.Errors.Select(e => e.Description).ToList(),
                        success = false
                    };
                }
               
                var result = await signInUser(userdata.Email!,password);
                return result;
            }
            catch (Exception ex)
            {
                return new AuthResult()
                {
                    Error = new List<string>() { ex.Message },
                    success = false
                };
            }

        }
        // here where we verify the refresh token
        // and here i extract the  claims from the jwt token  but i dont know why the jti claim is not generated if i put claim of userId
        public async Task<AuthResult> getrefeshedtoken(string token, string refersh)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                var refreshValidationParams = _tokenValidationParameters.Clone();
                refreshValidationParams.ValidateLifetime = false;
                refreshValidationParams.ValidateIssuer = false;
                ClaimsPrincipal tokeninverification = jwtHandler.
                    ValidateToken(token, refreshValidationParams,
                    out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtsecurtiy)
                {
                    bool result = jwtsecurtiy.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                var jwtSecurityToken = validatedToken as JwtSecurityToken;
                var claims = jwtSecurityToken?.Claims;
                var email = claims?.FirstOrDefault(c=>c.Type== JwtRegisteredClaimNames.Email)!.Value;
                var expiryClaim = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Expiration);
                var expiryDateTime = DateTime.Parse(expiryClaim.Value);
                if (expiryDateTime > DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        success = false,
                        Error = new List<string> { "Token is not expired yet." }
                    };
                }
                var storedToken = await _refershtokenReposiroty.GetRefreshTokenByIdAsync(refersh);
                if (storedToken == null || 
                    storedToken.isRevoked )
                {
                    return new AuthResult
                    {
                        success = false,
                        Error = new List<string> { "Invalid Token" }
                    };
                }
                if (storedToken.isUsed)
                {
                    return new AuthResult
                    {
                        success = false,
                        Error = new List<string> { "refesh Token is already used" }
                    };
                }
                if (storedToken.ExpiresDate < DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        success = false,
                        Error = new List<string> { "refesh Token is expired" }
                    };
                }
               
                var jti = jwtSecurityToken?.Id;
                if (storedToken.JwtId != jti)
                {
                    return new AuthResult
                    {
                        success = false,
                        Error = new List<string> { "Token doesnt match" }
                    };
                }
                storedToken.isUsed = true;
                await  _refershtokenReposiroty.UpdateRefreshTokenAsync(storedToken);
                var dbuser = await _userIdentityAuthincation.FindByEmailAsync(email);
                var roles = await _userIdentityAuthincation.GetUserRolesAsync(dbuser);
                return await GenerateJwtToken(dbuser,roles);

            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    success = false,
                    Error = new List<string> { ex.Message }
                };
            }
        }
        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            var datetimeval = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            datetimeval = datetimeval.AddSeconds(utcExpiryDate).ToLocalTime();
            return datetimeval;
        }
        public async Task<AuthResult> signInUser(string userEmail, string password)
        {
            try 
            {
                TUser? user = await _userIdentityAuthincation.signin(userEmail,password);
                if (user == null)
                {
                    return new AuthResult()
                    {
                        Error = new List<string>() { "Invalid email or password" },
                        success = false
                    };
                }
                var roles = await _userIdentityAuthincation.GetUserRolesAsync(user);
                if (roles == null || roles.Count == 0)
                {
                    return new AuthResult()
                    {
                        Error = new List<string>() { "User has no roles assigned" },
                        success = false
                    };
                }
                var result = await GenerateJwtToken(user, roles);
                return result;
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    return new AuthResult()
                    {
                        Error = new List<string>() { ex.Message },
                        success = false
                    };
                }
                else 
                {
                    return new AuthResult()
                    {
                        Error = new List<string>() { "An error occurred while signing in",ex.Message },
                        success = false
                    };

                }
            }
           
        }
        // here wen we generate the jwt token we also generate the refresh token
        // here is where error happens when  i try give claim Uid and give userId some how it not genrates jti claim
        // and also i tried to add claim of expiry date it also not working
        private async Task<AuthResult> GenerateJwtToken(IdentityUser user, List<string> roles)
        {
     
            var expirydateTime = DateTime.UtcNow.AddMinutes(30); // Set the token expiration time
            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["jwt:Key"]);
            var guid = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,guid ),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim(ClaimTypes.Expiration,expirydateTime.ToString() ));
            claims.Add(new Claim("UID", user.Id.ToString()));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["jwt:Issuer"],
                Audience = _configuration["jwt:audience"],

                Expires = expirydateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtHandler.CreateToken(tokenDescriptor);
            var jwttoken = jwtHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                Token = RandomString(35) + Guid.NewGuid(),
                JwtId = token.Id,
                isUsed = false,
                isRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiresDate = DateTime.UtcNow.AddMinutes(60)
            };
          
             await _refershtokenReposiroty.AddRefreshTokenAsync(refreshToken);
            // Implementation for generating JWT token goes here
            // This is a placeholder and should be replaced with actual token generation logic
            return new AuthResult()
            {
                Token = jwttoken,
                success = true,
                RefreshToken = refreshToken.Token
            };
        }
        private string RandomString(int lenght)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(chars, lenght).
                Select(x => x[random.Next(x.Length)]).ToArray());
        }

       
    }
}
