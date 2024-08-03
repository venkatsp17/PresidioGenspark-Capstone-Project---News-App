
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        ILog log = LogManager.GetLogger(typeof(AuthenticationController));
        [ExcludeFromCodeCoverage]
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        //[HttpPost("CreatePassword")]
        //[ProducesResponseType(typeof(RegisterReturnDTO), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> CreatePassword(CreatePasswordDTO createPasswordDTO)
        //{
        //    try
        //    {
        //        var user = await _authenticationService.CreatePassword(createPasswordDTO);
        //        return Ok(user);
        //    }
        //    catch (UnableToAddUserException ex)
        //    {
        //        return UnprocessableEntity(new ErrorModel(422, ex.Message));
        //    }
        //    catch (UnableToUpdateItemException ex)
        //    {
        //        return UnprocessableEntity(new ErrorModel(422, ex.Message));
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
        //    }
        //}

        //[HttpPost("login")]
        //[ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Login([FromBody] LoginGetDTO loginGetDTO)
        //{
        //    try
        //    {
        //        var user = await _authenticationService.LoginUser(loginGetDTO);
        //        return Ok(user);
        //    }
        //    catch (UnableToAddItemException ex)
        //    {
        //        return UnprocessableEntity(new ErrorModel(422, ex.Message));
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
        //    }
        //}

        //[HttpPost("logout")]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Logout([FromBody] string userId)
        //{
        //    try
        //    {
        //        await _authenticationService.LogoutUser(userId);
        //        return Ok();
        //    }
        //    catch (UnableToUpdateItemException ex)
        //    {
        //        return UnprocessableEntity(new ErrorModel(422, ex.Message));
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
        //    }
        //}

        [ExcludeFromCodeCoverage]
        [HttpPost("UserLogin")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserLogin(LoginGetDTO1 userLoginDTO)
        {
            log.Info($"UserLogin called with parameters: {userLoginDTO}");
            try
            {
                var result = await _authenticationService.UserLogin(userLoginDTO);
                log.Info($"UserLogin successful for user: {userLoginDTO.Email}");
                return Ok(result);
            }
            catch (UnauthorizedUserException ex)
            {
                log.Warn($"UnauthorizedUserException in UserLogin: {ex.Message}");
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (UnableToLoginException ex)
            {
                log.Warn($"UnableToLoginException in UserLogin: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (UserNotFoundException ex)
            {
                log.Warn($"UserNotFoundException in UserLogin: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in UserLogin", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }
        [ExcludeFromCodeCoverage]
        [HttpPost("UserRegister")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegisterReturnDTO>> UserRegister(RegisterGetDTO registerDTO)
        {
            log.Info($"UserRegister called with parameters: Email: {registerDTO.Email} Name: {registerDTO.Name}");
            try
            {
                var result = await _authenticationService.UserRegister(registerDTO);
                log.Info($"UserRegister successful for user: {registerDTO.Email}");
                return Ok(result);
            }
            catch (UserAlreadyExistsException ex)
            {
                log.Warn($"UserAlreadyExistsException in UserRegister: {ex.Message}");
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (UnableToRegisterException ex)
            {
                log.Warn($"UnableToRegisterException in UserRegister: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in UserRegister", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }
    }
}
