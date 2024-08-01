using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Services.Interfaces;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPreferenceController : ControllerBase
    {
        private readonly IUserPreferenceService _userPreferenceService;
        public UserPreferenceController(IUserPreferenceService userPreferenceService) {
            _userPreferenceService = userPreferenceService;
        }

        [Authorize]
        [HttpPost("addpreferences")]
        [ProducesResponseType(typeof(IEnumerable<UserPreferenceReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCategory(UserPreferenceDTO userPreferenceDTO)
        {
            try
            {
                var userPreferenceReturnDTOs = await _userPreferenceService.AddPreferences(userPreferenceDTO);
                return Ok(userPreferenceReturnDTOs);
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

        [Authorize]
        [HttpGet("getpreferences")]
        [ProducesResponseType(typeof(IEnumerable<UserPreferenceReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserPreferences(int userid)
        {
            try
            {
                var userPreferenceReturnDTOs = await _userPreferenceService.GetUserPreferences(userid);
                return Ok(userPreferenceReturnDTOs);
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
    }
}
