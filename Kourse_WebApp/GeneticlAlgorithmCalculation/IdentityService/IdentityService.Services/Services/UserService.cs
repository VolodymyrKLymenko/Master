using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Common.Persistence.Repositories;
using IdentityService.Services.Interfaces;

namespace IdentityService.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ISecurityService _securityService;
        private readonly IUserRepository _userRepository;

        public UserService(
            ISecurityService securityService,
            IUserRepository userRepository)
        {
            _securityService = securityService;
            _userRepository = userRepository;
        }

        public (string, User) AuthorizeUser(string login, string password)
        {
            var user = _userRepository.Get(u => u.Email == login).SingleOrDefault();
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Wrong password or email");
            }

            if (password != _securityService.DecryptPassword(user.PasswordHash, "salt"))
            {
                throw new ArgumentOutOfRangeException("Wrong password or email");
            }

            var accessToken = _securityService.GenerateAccessToken(user, TimeSpan.FromHours(1));

            return (accessToken, user);
        }

        public User CreateUser(string email, string password, string userLogin)
        {
            var passwordHash = _securityService.EncryptPassword(password, "salt");
            var user =
                new User
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    UserLogin = userLogin
                };

            ValidateUser(user);

            var existingUser = _userRepository
                .Get(u => u.Email == user.Email)
                .FirstOrDefault();

            if (existingUser != null)
            {
                throw new ArgumentOutOfRangeException("Already used email");
            }

            return _userRepository.Create(user);
        }

        public User GetUser(IEnumerable<Claim> claims)
        {
            if (!CheckUserIdInClaims(claims, out int userId))
            {
                throw new ArgumentOutOfRangeException("Not authorized to update user");
            }

            var user = _userRepository.FindById(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException($"Cannot find user with id {userId}");
            }

            return user;
        }

        public void RemoveUser(int id)
        {
            var user = _userRepository.FindById(id);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Cacnot find appropriate user");
            }

            _userRepository.Remove(user);
        }

        public User UpdateUser(IEnumerable<Claim> claims, string newEmail, string newPassword, string newUserLogin)
        {
            if (!CheckUserIdInClaims(claims, out int userId))
            {
                throw new ArgumentOutOfRangeException("Not authorized to update user");
            }

            var user = _userRepository.FindById(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Cannot find user to update");
            }

            user.Email = newEmail;
            user.UserLogin = newUserLogin;

            var newPasswordHash = _securityService.EncryptPassword(newPassword, "salt");
            user.PasswordHash = newPasswordHash;

            ValidateUser(user);

            var existingUser = _userRepository
                .Get(u => u.Email == newEmail)
                .FirstOrDefault();

            if (existingUser != null)
            {
                throw new ArgumentOutOfRangeException("Already used email");
            }

            _userRepository.Update(user);

            return user;
        }

        private static void ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email)) // add logic for checking email regex
            {
                throw new ArgumentException("Bad formatted email");
            }

            if (string.IsNullOrWhiteSpace(user.UserLogin))
            {
                throw new ArgumentException("Bad formatted user login");
            }
        }

        private bool CheckUserIdInClaims(IEnumerable<Claim> claims, out int userId)
        {
            return Int32.TryParse(
                claims.Single(c => c.Type == "UserId").Value,
                out userId);
        }
    }
}
