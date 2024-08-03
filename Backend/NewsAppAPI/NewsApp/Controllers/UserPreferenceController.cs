using log4net;
using Microsoft.AspNetCore.Authorization;
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
    public class UserPreferenceController : ControllerBase
    {
        private readonly IUserPreferenceService _userPreferenceService;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserPreferenceController));
        [ExcludeFromCodeCoverage]
        public UserPreferenceController(IUserPreferenceService userPreferenceService) {
            _userPreferenceService = userPreferenceService;
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("addpreferences")]
        [ProducesResponseType(typeof(IEnumerable<UserPreferenceReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCategory(UserPreferenceDTO userPreferenceDTO)
        {
            log.Info("AddCategory called");

            try
            {
                var userPreferenceReturnDTOs = await _userPreferenceService.AddPreferences(userPreferenceDTO);
                log.Info("Preferences added successfully");
                return Ok(userPreferenceReturnDTOs);
            }
            catch (UnableToAddItemException ex)
            {
                log.Warn("Unable to add item exception", ex);
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error occurred", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("getpreferences")]
        [ProducesResponseType(typeof(IEnumerable<UserPreferenceReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserPreferences(int userid)
        {
            log.Info($"GetUserPreferences called with userid: {userid}");

            try
            {
                var userPreferenceReturnDTOs = await _userPreferenceService.GetUserPreferences(userid);
                log.Info("User preferences retrieved successfully");
                return Ok(userPreferenceReturnDTOs);
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn("No available item exception", ex);
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error occurred", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }
    }
}
