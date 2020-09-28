using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GeneticlAlgorithmCalculation.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;

        public AccountController(
            IUserService userService,
            ILogger<AccountController> logger)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("accounts/login")]
        public ActionResult<AuthenticationResponse> Authenticate(AuthenticateRequest request)
        {
            var (accessTokem, user) = _userService.AuthorizeUser(request.Email, request.Password);

            return
                new AuthenticationResponse
                {
                    AccessToken = accessTokem,
                    UserId = user.Id,
                    Email = user.Email
                };
        }

        public class AuthenticateRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class AuthenticationResponse
        {
            public string AccessToken { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }
        }

        [HttpGet("accounts/users")]
        public ActionResult<UserResponse> GetUser()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var user =_userService.GetUser(identity.Claims);

            return
                new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Login = user.UserLogin
                };
        }

        public class UserResponse
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Login { get; set; }
        }

        [HttpPost("accounts/users")]
        public IActionResult CreateUser(RegistrationRequest request)
        {
            _userService.CreateUser(request.Email, request.Password, request.UserLogin);

            return Ok();
        }

        public class RegistrationRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string UserLogin { get; set; }
        }

        [HttpPut("accounts/users")]
        public IActionResult UpdateUser(UpdateUserRequest request)
        {
            var identity = (ClaimsIdentity)User.Identity;

            _userService.UpdateUser(identity.Claims, request.NewEmail, request.NewPassword, request.NewUserLogin);

            return Ok();
        }

        public class UpdateUserRequest
        {
            public string NewEmail { get; set; }
            public string NewPassword { get; set; }
            public string NewUserLogin { get; set; }
        }
    }
}
