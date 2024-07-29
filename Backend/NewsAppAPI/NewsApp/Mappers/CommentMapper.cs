using NewsApp.DTOs;
using NewsApp.Models;

namespace NewsApp.Mappers
{
    public class CommentMapper
    {

        public static CommentReturnDTO MapCommentReturnDTO(Comment comment)
        {
            return new CommentReturnDTO
            {
                ArticleID = comment.ArticleID,
                Content = comment.Content,
                CommentID = comment.CommentID,
                Timestamp = comment.Timestamp,
                UserName = comment.User.Name,
            };
        }
    }
}
