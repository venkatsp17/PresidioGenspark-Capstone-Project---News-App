using Microsoft.AspNetCore.Authorization;
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
        private readonly IRankArticleService _rankarticleService;

        public ArticleController(IArticleService articleService, IRankArticleService rankarticleService)
        {
            _articleService = articleService;
            _rankarticleService = rankarticleService;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("topstories")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TopStories(int categoryID, string status, int pageno=1, int pagesize=10)
        {
            try
            {
                var articles = await _articleService.GetPaginatedArticlesAsync(pageno, pagesize, status, categoryID);
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPut("editArticleDetails")]
        [ProducesResponseType(typeof(AdminArticleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditArticleDetails(AdminArticleEditGetDTO adminArticleReturnDTO)
        {
            try
            {
                var article = await _articleService.EditArticleData(adminArticleReturnDTO);
                return Ok(article);
            }
            catch (ItemNotFoundException ex)
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


        [HttpGet("userpaginatedarticles")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserPaginatedArticles(int categoryID, int userid, int pageno = 1, int pagesize = 10)
        {
            try
            {
                var articles = await _articleService.GetPaginatedArticlesForUserAsync(pageno, pagesize, categoryID, userid);    
                return Ok(articles);
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }

        [HttpGet("userfeeds")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserFeeds(int userid, int pageno = 1, int pagesize = 10)
        {
            try
            {
                var articles = await _articleService.GetPaginatedFeedsForUserAsync(pageno, pagesize,  userid);
                return Ok(articles);
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }

        [Authorize]
        [HttpPost("articlesharecount")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShareCount(ShareDataDTO shareDataDTO)
        {
            try
            {
                await _articleService.UpdateShareCount(shareDataDTO);
                return Ok("Update Successfull!");
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }

        [Authorize]
        [HttpGet("rankedarticles")]
        [ProducesResponseType(typeof(IEnumerable<AdminArticleReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RankArticles(int categoryid, int userid)
        {
            try
            {
                var articles = await _rankarticleService.RankTop3Articles(categoryid, userid);
                return Ok(articles);
            }
            catch (UnableToAddItemException ex)
            {
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }

    }
}
