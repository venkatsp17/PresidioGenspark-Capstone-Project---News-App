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
    public class CommentController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommentController));
        private readonly ICommentService _commentService;
        [ExcludeFromCodeCoverage]
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("postcomment")]
        public async Task<IActionResult> PostComment([FromBody] CommentDTO comment)
        {
            log.Info("PostComment called");

            if (comment == null)
            {
                log.Warn("Comment is null");
                return BadRequest("Comment cannot be null.");
            }

            comment.Timestamp = DateTime.UtcNow;

            try
            {
                await _commentService.PostComment(comment);
                log.Info("Comment posted successfully");
                return Ok();
            }
            catch (UnableToAddItemException ex)
            {
                log.Error("Unable to add item exception", ex);
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
        [HttpGet("getcommentsByID")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentsByID(string id, string type)
        {
            log.Info($"GetCommentsByID called with id: {id}, type: {type}");

            try
            {
                var articles = await _commentService.GetAllCommentsByArticleID(id, type);
                log.Info("Comments retrieved successfully");
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
