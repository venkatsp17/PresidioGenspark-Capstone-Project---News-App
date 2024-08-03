using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
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
    public class SavedArticleServiceTest
    {
        private Mock<IHubContext<CommentHub>> _hubContextMock;
        private Mock<ISavedRepository> _savedArticleRepositoryMock;
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<ILogger<SavedArticleService>> _loggerMock;
        private SavedArticleService _savedArticleService;

        [SetUp]
        public void SetUp()
        {
            _hubContextMock = new Mock<IHubContext<CommentHub>>();
            _savedArticleRepositoryMock = new Mock<ISavedRepository>();
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _loggerMock = new Mock<ILogger<SavedArticleService>>();
            _savedArticleService = new SavedArticleService(
                _hubContextMock.Object,
                _savedArticleRepositoryMock.Object,
                _articleRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        // Test for CheckForSaved method
        [Test]
        public async Task CheckForSaved_ShouldReturnTrue_WhenArticleIsSaved()
        {
            // Arrange
            var articleid = 1;
            var userid = 1;
            var savedArticle = new SavedArticle { ArticleID = articleid, UserID = userid };

            _savedArticleRepositoryMock.Setup(repo => repo.GetBy2Id("ArticleID", articleid.ToString(), "UserID", userid.ToString()))
                .ReturnsAsync(savedArticle);

            // Act
            var result = await _savedArticleService.CheckForSaved(articleid, userid);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task CheckForSaved_ShouldReturnFalse_WhenArticleIsNotSaved()
        {
            // Arrange
            var articleid = 1;
            var userid = 1;

            _savedArticleRepositoryMock.Setup(repo => repo.GetBy2Id("ArticleID", articleid.ToString(), "UserID", userid.ToString()))
                .ReturnsAsync((SavedArticle)null);

            // Act
            var result = await _savedArticleService.CheckForSaved(articleid, userid);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SaveAndUnSaveArticle_ShouldThrowUnableToUpdateItemException_WhenDeleteFails()
        {
            // Arrange
            var articleid = 1;
            var userid = 1;
            var savedArticle = new SavedArticle { SavedArticleID = 1, ArticleID = articleid, UserID = userid };
            var article = new Article { ArticleID = articleid, SaveCount = 1 };

            _savedArticleRepositoryMock.Setup(repo => repo.GetBy2Id("ArticleID", articleid.ToString(), "UserID", userid.ToString()))
                .ReturnsAsync(savedArticle);
            _savedArticleRepositoryMock.Setup(repo => repo.Delete(savedArticle.SavedArticleID.ToString())).ReturnsAsync((SavedArticle)null);
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", articleid.ToString())).ReturnsAsync(article);

            // Act & Assert
            Assert.ThrowsAsync<UnableToUpdateItemException>(async () => await _savedArticleService.SaveAndUnSaveArticle(articleid, userid));
        }

        // Test for GetAllSavedArticles method
        [Test]
        public async Task GetAllSavedArticles_ShouldReturnPaginatedArticles_WhenArticlesAreAvailable()
        {
            // Arrange
            var userid = 1;
            var pageNumber = 1;
            var pageSize = 2;
            var query = "sample";
            var savedArticles = new List<SavedArticle>
            {
                new SavedArticle { ArticleID = 1, UserID = userid },
                new SavedArticle { ArticleID = 2, UserID = userid }
            };
            var article1 = new Article { ArticleID = 1, Title = "Sample Article 1", Content = "Content 1", CreatedAt = DateTime.Now };
            var article2 = new Article { ArticleID = 2, Title = "Sample Article 2", Content = "Content 2", CreatedAt = DateTime.Now.AddMinutes(-5) };

            _savedArticleRepositoryMock.Setup(repo => repo.GetAll("UserID", userid.ToString())).ReturnsAsync(savedArticles);
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", "1")).ReturnsAsync(article1);
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", "2")).ReturnsAsync(article2);

            // Act
            var result = await _savedArticleService.GetAllSavedArticles(userid, pageNumber, pageSize, query);

            // Assert
            Assert.AreEqual(2, result.Articles.Count());
            Assert.AreEqual(1, result.totalpages);
        }

        [Test]
        public void GetAllSavedArticles_ShouldThrowNoAvailableItemException_WhenNoSavedArticlesAreAvailable()
        {
            // Arrange
            var userid = 1;
            var pageNumber = 1;
            var pageSize = 2;
            var query = "sample";

            _savedArticleRepositoryMock.Setup(repo => repo.GetAll("UserID", userid.ToString())).ReturnsAsync(new List<SavedArticle>());

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _savedArticleService.GetAllSavedArticles(userid, pageNumber, pageSize, query));
        }
    }
}
