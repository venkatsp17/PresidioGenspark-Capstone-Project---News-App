using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [Authorize]
        [HttpPost("postcomment")]
        public async Task<IActionResult> PostComment([FromBody] CommentDTO comment)
        {
            if (comment == null)
            {
                return BadRequest("Comment cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            comment.Timestamp = DateTime.UtcNow;

            try
            {
                await _commentService.PostComment(comment);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("getcommentsByID")]
        [ProducesResponseType(typeof(AdminArticlePaginatedReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentsByID(string id, string type)
        {
            try
            {
                var articles = await _commentService.GetAllCommentsByArticleID(id, type);
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
    }
}
