
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Models;
using NewsApp.Services.Interfaces;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginGetDTO loginGetDTO)
        {
            var user = await _authenticationService.LoginUser(loginGetDTO);
            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string userId)
        {
            await _authenticationService.LogoutUser(userId);
            return Ok();
        }
    }
}
