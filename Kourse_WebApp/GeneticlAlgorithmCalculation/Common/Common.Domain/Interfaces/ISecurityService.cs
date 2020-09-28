using Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Common.Domain.Interfaces
{
    public interface ISecurityService
    {
        string GenerateAccessToken(User user, TimeSpan expirationDuration);
        int FetchUserId(IEnumerable<Claim> claims);
        string EncryptPassword(string password, string salt);
        string DecryptPassword(string passwordHash, string salt);
    }
}
