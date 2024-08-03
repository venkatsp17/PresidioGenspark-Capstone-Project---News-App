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
    public class SavedArticleController : ControllerBase
    {
        private readonly ISavedArticleService _savedArticleService;
        private static readonly ILog log = LogManager.GetLogger(typeof(SavedArticleController));
        [ExcludeFromCodeCoverage]
        public SavedArticleController(ISavedArticleService savedArticleService)
        {
            _savedArticleService = savedArticleService;
        }
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPut("savearticle")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveArticle(int articleid, int userid)
        {
            log.Info($"SaveArticle called with articleid: {articleid}, userid: {userid}");

            try
            {
                var article = await _savedArticleService.SaveAndUnSaveArticle(articleid, userid);
                log.Info("Article saved/unsaved successfully");
                return Ok(article);
            }
            catch (UnableToAddItemException ex)
            {
                log.Warn("Unable to add item exception", ex);
                return UnprocessableEntity(new ErrorModel(422, ex.Message));
            }
            catch (UnableToUpdateItemException ex)
            {
                log.Warn("Unable to update item exception", ex);
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
        [HttpGet("getallarticlesbyid")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllArticlesByID(int userid, int pageno, int pagesize, string query="null")
        {
            log.Info($"GetAllArticlesByID called with userid: {userid}, pageno: {pageno}, pagesize: {pagesize}, query: {query}");

            try
            {
                var articles = await _savedArticleService.GetAllSavedArticles(userid, pageno, pagesize, query);
                log.Info("Articles retrieved successfully");
                return Ok(articles);
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
