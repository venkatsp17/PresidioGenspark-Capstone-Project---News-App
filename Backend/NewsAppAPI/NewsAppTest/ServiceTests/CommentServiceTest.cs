using Microsoft.AspNetCore.SignalR;
using Moq;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAppTest.ServiceTests
{
    [TestFixture]
    public class CommentServiceTest
    {
        private Mock<IHubContext<CommentHub>> _hubContextMock;
        private Mock<IRepository<string, Comment, string>> _commentRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IArticleRepository> _articleRepositoryMock;
        private CommentService _commentService;

        [SetUp]
        public void SetUp()
        {
            _hubContextMock = new Mock<IHubContext<CommentHub>>();
            _commentRepositoryMock = new Mock<IRepository<string, Comment, string>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _commentService = new CommentService(
                _hubContextMock.Object,
                _commentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _articleRepositoryMock.Object
            );
        }

        [Test]
        public async Task PostComment_ShouldAddComment_WhenCommentIsValid()
        {
            // Arrange
            var commentDTO = new CommentDTO
            {
                Content = "Great article!",
                ArticleID = 123,
                UserID = 1,
                Timestamp = DateTime.Now
            };
            var user = new User { UserID = 1, Name = "John Doe" };
            var article = new Article { ArticleID = 123, CommentCount = 0 };
            var newComment = new Comment
            {
                CommentID = 1,
                Content = "Great article!",
                ArticleID = 123,
                UserID = 1,
                Timestamp = DateTime.Now
            };

            _userRepositoryMock.Setup(repo => repo.Get("UserID", commentDTO.UserID.ToString()))
                               .ReturnsAsync(user);
            _commentRepositoryMock.Setup(repo => repo.Add(It.IsAny<Comment>()))
                                  .ReturnsAsync(newComment);
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", commentDTO.ArticleID.ToString()))
                                  .ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString()))
                                  .ReturnsAsync(article);

            var mockClientProxy = new Mock<IClientProxy>();
            _hubContextMock.Setup(hub => hub.Clients.Group(It.IsAny<string>()))
                           .Returns(mockClientProxy.Object);

            // Act
            await _commentService.PostComment(commentDTO);

            // Assert
            _commentRepositoryMock.Verify(repo => repo.Add(It.IsAny<Comment>()), Times.Once);
        
        }

        [Test]
        public void PostComment_ShouldThrowUnableToAddItemException_WhenUserNotFound()
        {
            // Arrange
            var commentDTO = new CommentDTO { Content = "Great article!", ArticleID = 123, UserID = 1, Timestamp = DateTime.Now };

            _userRepositoryMock.Setup(repo => repo.Get("UserID", commentDTO.UserID.ToString())).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToAddItemException>(async () => await _commentService.PostComment(commentDTO));
            _userRepositoryMock.Verify(repo => repo.Get("UserID", commentDTO.UserID.ToString()), Times.Once);
        }

        [Test]
        public void PostComment_ShouldThrowUnableToAddItemException_WhenCommentNotAdded()
        {
            // Arrange
            var commentDTO = new CommentDTO { Content = "Great article!", ArticleID = 123, UserID = 1, Timestamp = DateTime.Now };
            var user = new User { UserID = 1, Name = "John Doe" };

            _userRepositoryMock.Setup(repo => repo.Get("UserID", commentDTO.UserID.ToString())).ReturnsAsync(user);
            _commentRepositoryMock.Setup(repo => repo.Add(It.IsAny<Comment>())).ReturnsAsync((Comment)null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToAddItemException>(async () => await _commentService.PostComment(commentDTO));
            _userRepositoryMock.Verify(repo => repo.Get("UserID", commentDTO.UserID.ToString()), Times.Once);
            _commentRepositoryMock.Verify(repo => repo.Add(It.IsAny<Comment>()), Times.Once);
        }

        [Test]
        public async Task GetAllCommentsByArticleID_ShouldReturnComments_WhenCommentsExist()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment { CommentID = 1, Content = "Great article!", ArticleID = 123, UserID = 1, Timestamp = DateTime.Now,User = new User(){Name="Test123" } },
                new Comment { CommentID = 2, Content = "Very informative.", ArticleID = 123, UserID = 2, Timestamp = DateTime.Now ,User = new User(){Name="Test123" }  }
            };

            _commentRepositoryMock.Setup(repo => repo.GetAll("ArticleID", "123")).ReturnsAsync(comments);

            // Act
            var result = await _commentService.GetAllCommentsByArticleID("123", "ArticleID");

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Great article!", result.First().Content);
            _commentRepositoryMock.Verify(repo => repo.GetAll("ArticleID", "123"), Times.Once);
        }

        [Test]
        public void GetAllCommentsByArticleID_ShouldThrowNoAvailableItemException_WhenNoCommentsExist()
        {
            // Arrange
            _commentRepositoryMock.Setup(repo => repo.GetAll("", "123")).ReturnsAsync((IEnumerable<Comment>)null);

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _commentService.GetAllCommentsByArticleID("123", ""));
            _commentRepositoryMock.Verify(repo => repo.GetAll("", "123"), Times.Once);
        }
    }
}
