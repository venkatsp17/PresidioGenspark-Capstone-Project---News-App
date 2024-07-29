using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Mappers;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;

namespace NewsApp.Services.Classes
{
    public class CommentService : ICommentService
    {
        private readonly IHubContext<CommentHub> _hubContext;
        private readonly IRepository<string, Comment, string> _commentRepository;
        private readonly IRepository<string, User, string> _userRepository;

        public CommentService(IHubContext<CommentHub> hubContext, IRepository<string, Comment, string> commentRepository, IRepository<string, User, string> userRepository)
        {
            _hubContext = hubContext;
            _commentRepository =commentRepository;
            _userRepository =userRepository;
        }

        public async Task PostComment(CommentDTO comment)
        {
            
            var newComment = new Comment
            {
                Content = comment.Content,
                ArticleID = comment.ArticleID,
                UserID = comment.UserID,
                Timestamp = comment.Timestamp,
            };
            var user = await _userRepository.Get("UserID", comment.UserID.ToString());

            if (user == null)
            {
                throw new UnableToAddItemException();
            }

            var result = await _commentRepository.Add(newComment);
            if (result == null)
            {
                throw new UnableToAddItemException();
            }

            // Broadcast to SignalR clients

            try
            {
                var commentTOsend = new CommentReturnDTO()
                {
                    CommentID=result.CommentID,
                    Content=result.Content,
                    ArticleID=result.ArticleID,
                    Timestamp=result.Timestamp,
                    UserName= user.Name,
                };
                await _hubContext.Clients.Group(comment.ArticleID.ToString()).SendAsync("ReceiveComment",commentTOsend);
                Console.WriteLine("Comment broadcasted successfully.");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error broadcasting comment: {ex.Message}");
            }
        }


        public async Task<IEnumerable<CommentReturnDTO>> GetAllCommentsByArticleID (string id, string t)
        {
            var comments = await _commentRepository.GetAll(t, id);
            
            if(comments == null)
            {
                throw new NoAvailableItemException();
            }

            return comments.Select(c=>CommentMapper.MapCommentReturnDTO(c));
        }

    }
}
