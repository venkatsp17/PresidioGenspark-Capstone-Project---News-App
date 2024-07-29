using NewsApp.DTOs;
using NewsApp.Models;

namespace NewsApp.Services.Interfaces
{
    public interface ICommentService
    {
        Task PostComment(CommentDTO comment);

        Task<IEnumerable<CommentReturnDTO>> GetAllCommentsByArticleID(string articleID, string type);
    }
}
