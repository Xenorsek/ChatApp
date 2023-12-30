using ChatApp.Common.ConfigurationHelpers;
using ChatApp.Core;
using ChatApp.Core.Interfaces;
using ChatApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.API.Controllers
{
    [EnableRateLimiting("LoginLimiter")]
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest model)
        {
            var loginInfo = await _authorizationService.Login(model.LoginProvider, model.Password);
            if (loginInfo is not null)
            {
                return Ok(loginInfo);
            }
            else
            {
                return Unauthorized();
            }
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest model) {
            var result = await _authorizationService.Register(model.UserName, model.Email, model.Password);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }       
    }
}
