using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private static readonly ILog log = LogManager.GetLogger(typeof(CategoryController));
        [ExcludeFromCodeCoverage]
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [ExcludeFromCodeCoverage]
        [HttpGet("getAllCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            log.Info("GetAllCategories called.");
            try
            {
                var categories = await _categoryService.GetAllCategories();
                log.Info("GetAllCategories successful.");
                return Ok(categories);
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn($"NoAvailableItemException in GetAllCategories: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in GetAllCategories.", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("getAllAdminCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryAdminDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAdminCategories(int articleid=0)
        {
            log.Info($"GetAllAdminCategories called with articleid: {articleid}");
            try
            {
                var categories = await _categoryService.GetAllAdminCategories(articleid);
                log.Info("GetAllAdminCategories successful.");
                return Ok(categories);
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn($"NoAvailableItemException in GetAllAdminCategories: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in GetAllAdminCategories.", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Admin")]
        [HttpPost("addcategory")]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCategory(CategoryGetDTO categoryGetDTO)
        {
            log.Info($"AddCategory called with categoryGetDTO: {categoryGetDTO}");
            try
            {
                var category = await _categoryService.AddCategory(categoryGetDTO);
                log.Info("AddCategory successful.");
                return Ok(category);
            }
            catch (ItemAlreadyExistException ex)
            {
                log.Warn($"ItemAlreadyExistException in AddCategory: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (UnableToAddItemException ex)
            {
                log.Warn($"UnableToAddItemException in AddCategory: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in AddCategory.", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Admin")]
        [HttpPut("removecategory")]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveCategory(int categoryid)
        {
            log.Info($"RemoveCategory called with categoryid: {categoryid}");
            try
            {
                var category = await _categoryService.DeleteCategory(categoryid);
                log.Info("RemoveCategory successful.");
                return Ok(category);
            }
            catch (ItemNotFoundException ex)
            {
                log.Warn($"ItemNotFoundException in RemoveCategory: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in RemoveCategory.", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred."));
            }
        }
    }
}
