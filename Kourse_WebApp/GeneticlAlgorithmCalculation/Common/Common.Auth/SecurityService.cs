using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Common.Auth
{
    public static class SecurityConstants
    {
        public const string UserIdClaimName = "UserId";
    }

    public class SecurityService : ISecurityService
    {
        public string GenerateAccessToken(User user, TimeSpan expirationDuration)
        {
            var claims = new[]
            {
                new Claim(SecurityConstants.UserIdClaimName, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthenticationExtensions.AuthSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.Add(expirationDuration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int FetchUserId(IEnumerable<Claim> claims)
        {
            if (!int.TryParse(claims.FirstOrDefault(claim => claim.Type == SecurityConstants.UserIdClaimName)?.Value, out var userId))
            {
                throw new UnauthorizedAccessException();
            }

            return userId;
        }

        public string EncryptPassword(string password, string salt)
        {
            // TODO Implement this
            return password + salt;
        }

        public string DecryptPassword(string passwordHash, string salt)
        {
            // TO DO implement password hashing
            return passwordHash.Replace(salt, "");
        }
    }

}
