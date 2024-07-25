
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Exceptions;
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
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginGetDTO loginGetDTO)
        {
            try
            {
                var user = await _authenticationService.LoginUser(loginGetDTO);
                return Ok(user);
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] string userId)
        {
            try
            {
                await _authenticationService.LogoutUser(userId);
                return Ok();
            }
            catch (UnableToUpdateItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }


        [HttpPost("UserLogin")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserLogin(LoginGetDTO1 userLoginDTO)
        {
            try
            {
                var result = await _authenticationService.UserLogin(userLoginDTO);
                return Ok(result);
            }
            catch (UnauthorizedUserException ex)
            {
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (UnableToLoginException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }

        [HttpPost("UserRegister")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegisterReturnDTO>> UserRegister(RegisterGetDTO registerDTO)
        {
            try
            {
                var result = await _authenticationService.UserRegister(registerDTO);
                return Ok(result);
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (UnableToRegisterException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }
    }
}
