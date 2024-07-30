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
    public class SavedArticleController : ControllerBase
    {
        private readonly ISavedArticleService _savedArticleService;

        public SavedArticleController(ISavedArticleService savedArticleService)
        {
            _savedArticleService = savedArticleService;
        }

        [Authorize]
        [HttpPut("savearticle")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveArticle(int articleid, int userid)
        {
            try
            {
                var article = await _savedArticleService.SaveAndUnSaveArticle(articleid, userid);
                return Ok(article);
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
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



        [Authorize]
        [HttpGet("getallarticlesbyid")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllArticlesByID(int userid, int pageno, int pagesize, string query="null")
        {
            try
            {
                var article = await _savedArticleService.GetAllSavedArticles(userid, pageno, pagesize, query);
                return Ok(article);
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
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
    }
}
