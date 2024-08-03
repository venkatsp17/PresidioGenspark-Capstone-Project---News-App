using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using static NewsApp.Models.Enum;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IRankArticleService _rankarticleService;
        ILog log = LogManager.GetLogger(typeof(ArticleController));
        [ExcludeFromCodeCoverage]
        public ArticleController(IArticleService articleService, IRankArticleService rankarticleService)
        {
            _articleService = articleService;
            _rankarticleService = rankarticleService;
        }

        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Admin")]
        [HttpGet("topstories")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TopStories(int categoryID, string status, int pageno=1, int pagesize=10)
        {
            log.Info($"TopStories called with parameters: categoryID={categoryID}, status={status}, pageno={pageno}, pagesize={pagesize}");
            try
            {
                var articles = await _articleService.GetPaginatedArticlesAsync(pageno, pagesize, status, categoryID);
                log.Info($"Successfully retrieved articles for categoryID={categoryID}, status={status}, pageno={pageno}, pagesize={pagesize}");
                return Ok(articles);
            }
            catch (ArgumentException ex)
            {
                log.Warn($"ArgumentException in TopStories: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn($"NoAvailableItemException in TopStories: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in TopStories", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }

        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Admin")]
        [HttpPut("changeArticleStatus")]
        [ProducesResponseType(typeof(AdminArticleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeArticleStatus(string articleId, ArticleStatus articleStatus)
        {
            log.Info($"ChangeArticleStatus called with parameters: articleId={articleId}, articleStatus={articleStatus}");
            try
            {
                var article = await _articleService.ChangeArticleStatus(articleId, articleStatus);
                log.Info($"Successfully changed status for articleId={articleId} to {articleStatus}");
                return Ok(article);
            }
            catch (ItemNotFoundException ex)
            {
                log.Warn($"ItemNotFoundException in ChangeArticleStatus: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (UnableToUpdateItemException ex)
            {
                log.Warn($"UnableToUpdateItemException in ChangeArticleStatus: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in ChangeArticleStatus", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Admin")]
        [HttpPut("editArticleDetails")]
        [ProducesResponseType(typeof(AdminArticleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditArticleDetails(AdminArticleEditGetDTO adminArticleReturnDTO)
        {
            log.Info($"EditArticleDetails called with parameters: adminArticleReturnDTO={adminArticleReturnDTO}");
            try
            {
                var article = await _articleService.EditArticleData(adminArticleReturnDTO);
                log.Info($"Successfully edited article details for articleId={adminArticleReturnDTO.ArticleID}");
                return Ok(article);
            }
            catch (ItemNotFoundException ex)
            {
                log.Warn($"ItemNotFoundException in EditArticleDetails: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (UnableToUpdateItemException ex)
            {
                log.Warn($"UnableToUpdateItemException in EditArticleDetails: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in EditArticleDetails", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }

        [ExcludeFromCodeCoverage]
        [HttpGet("userpaginatedarticles")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserPaginatedArticles(int categoryID, int userid, int pageno = 1, int pagesize = 10)
        {
            log.Info($"UserPaginatedArticles called with parameters: categoryID={categoryID}, userid={userid}, pageno={pageno}, pagesize={pagesize}");
            try
            {
                var articles = await _articleService.GetPaginatedArticlesForUserAsync(pageno, pagesize, categoryID, userid);
                log.Info($"Successfully retrieved user paginated articles for categoryID={categoryID}, userid={userid}, pageno={pageno}, pagesize={pagesize}");
                return Ok(articles);
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn($"NoAvailableItemException in UserPaginatedArticles: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in UserPaginatedArticles", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("userfeeds")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserFeeds(int userid, int pageno = 1, int pagesize = 10)
        {
            log.Info($"UserFeeds called with parameters: userid={userid}, pageno={pageno}, pagesize={pagesize}");
            try
            {
                var articles = await _articleService.GetPaginatedFeedsForUserAsync(pageno, pagesize, userid);
                log.Info($"Successfully retrieved user feeds for userid={userid}, pageno={pageno}, pagesize={pagesize}");
                return Ok(articles);
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn($"NoAvailableItemException in UserFeeds: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in UserFeeds", ex);
                return StatusCode(500, new ErrorModel(500, $"An unexpected error occurred. {ex.Message}"));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Admin")]
        [HttpGet("dashboarddata")]
        [ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DashboardData()
        {
            log.Info("DashboardData called");
            try
            {
                var statistics = await _rankarticleService.GetAllStatistics();
                log.Info("Successfully retrieved dashboard data");
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in DashboardData", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("articlesharecount")]
        [ProducesResponseType(typeof(ShareDataReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShareCount(ShareDataDTO shareDataDTO)
        {
            log.Info($"UpdateShareCount called with parameters: shareDataDTO={shareDataDTO}");
            try
            {
                await _articleService.UpdateShareCount(shareDataDTO);
                log.Info("Successfully updated share count");
                return Ok("Update Successful!");
            }
            catch (UnableToAddItemException ex)
            {
                log.Warn($"UnableToAddItemException in UpdateShareCount: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (UnableToUpdateItemException ex)
            {
                log.Warn($"UnableToUpdateItemException in UpdateShareCount: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in UpdateShareCount", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("rankedarticles")]
        [ProducesResponseType(typeof(IEnumerable<AdminArticleReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RankArticles(int categoryid, int userid)
        {
            log.Info($"RankArticles called with parameters: categoryid={categoryid}, userid={userid}");
            try
            {
                var articles = await _rankarticleService.RankTop3Articles(categoryid, userid);
                log.Info($"Successfully ranked articles for categoryid={categoryid}, userid={userid}");
                return Ok(articles);
            }
            catch (NoAvailableItemException ex)
            {
                log.Warn($"NoAvailableItemException in RankArticles: {ex.Message}");
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in RankArticles", ex);
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred."));
            }
        }

    }
}
