using Common.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityService.Services.Interfaces
{
    public interface IUserService
    {
        (string, User) AuthorizeUser(string login, string password);
        User CreateUser(string email, string password, string userLogin);
        User GetUser(IEnumerable<Claim> claims);
        void RemoveUser(int id);
        User UpdateUser(IEnumerable<Claim> claims, string newEmail, string newPassword, string newUserLogin);
    }
}
