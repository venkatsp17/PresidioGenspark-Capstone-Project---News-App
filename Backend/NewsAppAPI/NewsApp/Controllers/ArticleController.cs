using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Services.Interfaces;
using static NewsApp.Models.Enum;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("topstories")]
        [ProducesResponseType(typeof(IEnumerable<AdminArticleReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TopStories(int pageno, int pagesize)
        {
            try
            {
                var articles = await _articleService.GetTopStoryArticlesAsync();
                return Ok(articles);
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

        [HttpPut("changeArticleStatus")]
        [ProducesResponseType(typeof(AdminArticleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeArticleStatus(string articleId, ArticleStatus articleStatus)
        {
            try
            {
                var article = await _articleService.ChangeArticleStatus(articleId, articleStatus);
                return Ok(article);
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
