using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NewsApp.Models.Enum;

namespace NewsAppTest.ServiceTests
{
    public class ArticleServiceTest
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<IUserPreferenceRepository> _userPreferenceRepositoryMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IArticleCategoryRepository> _articleCategoryRepositoryMock;
        private Mock<HttpClient> _httpClientMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Mock<ISavedArticleService> _savedArticleServiceMock;
        private Mock<IRepository<string, ShareData, string>> _shareDataRepositoryMock;
        private Mock<IHubContext<CommentHub>> _hubContextMock;
        private ArticleService _articleService;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _userPreferenceRepositoryMock = new Mock<IUserPreferenceRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _articleCategoryRepositoryMock = new Mock<IArticleCategoryRepository>();
            _httpClientMock = new Mock<HttpClient>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _savedArticleServiceMock = new Mock<ISavedArticleService>();
            _shareDataRepositoryMock = new Mock<IRepository<string, ShareData, string>>();
            _hubContextMock = new Mock<IHubContext<CommentHub>>();

            _articleService = new ArticleService(
                _articleRepositoryMock.Object,
                _httpClientMock.Object,
                _hubContextMock.Object,
                _userPreferenceRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _articleCategoryRepositoryMock.Object,
                _memoryCacheMock.Object,
                _savedArticleServiceMock.Object,
                _shareDataRepositoryMock.Object
            );
        }

        [Test]
        public async Task ChangeArticleStatus_ShouldChangeStatus_WhenArticleExists()
        {
            // Arrange
            var articleId = "123";
            var newStatus = ArticleStatus.Approved;
            var article = new Article { ArticleID = 123, Status = ArticleStatus.Pending };

            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", articleId)).ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString())).ReturnsAsync(article);

            // Act
            var result = await _articleService.ChangeArticleStatus(articleId, newStatus);

            // Assert
            Assert.AreEqual(newStatus, result.Status);
            _articleRepositoryMock.Verify(repo => repo.Get("ArticleID", articleId), Times.Once);
            _articleRepositoryMock.Verify(repo => repo.Update(article, article.ArticleID.ToString()), Times.Once);
        }

      

        [Test]
        public async Task ChangeArticleStatus_ShouldChangeStatus_ErrorUpdating()
        {
            // Arrange
            var articleId = "123";
            var newStatus = ArticleStatus.Approved;
            var article = new Article { ArticleID = 123, Status = ArticleStatus.Pending };

            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", articleId)).ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString())).ReturnsAsync((Article)null);


            // Assert
            Assert.ThrowsAsync<UnableToUpdateItemException>(async() => await _articleService.ChangeArticleStatus(articleId, newStatus));
        }

        [Test]
        public void ChangeArticleStatus_ShouldThrowItemNotFoundException_WhenArticleDoesNotExist()
        {
            // Arrange
            var articleId = "123";
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", articleId)).ReturnsAsync((Article)null);

            // Act & Assert
            Assert.ThrowsAsync<ItemNotFoundException>(() => _articleService.ChangeArticleStatus(articleId, ArticleStatus.Approved));
        }

        [Test]
        public async Task GetPaginatedArticlesAsync_ShouldReturnPaginatedArticles_WhenArticlesExist()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article { ArticleID = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Article { ArticleID = 2, CreatedAt = DateTime.UtcNow }
            };
            var status = "Pending";
            var categoryID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllByStatusAndCategoryAsync(ArticleStatus.Pending, categoryID)).ReturnsAsync(articles);

            // Act
            var result = await _articleService.GetPaginatedArticlesAsync(1, 1, status, categoryID);

            // Assert
            Assert.AreEqual(1, result.Articles.Count());
            Assert.AreEqual(2, result.totalpages);
        }

        [Test]
        public void GetPaginatedArticlesAsync_ShouldThrowNoAvailableItemException_WhenNoArticlesExist()
        {
            // Arrange
            var status = "Pending";
            var categoryID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllByStatusAndCategoryAsync(ArticleStatus.Pending, categoryID)).ReturnsAsync((IEnumerable<Article>)null);

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(() => _articleService.GetPaginatedArticlesAsync(1, 1, status, categoryID));
        }

        [Test]
        public void GetPaginatedArticlesAsync_ShouldThrowException_WrongStatus()
        {
            // Arrange
            var status = "Wrong";
            var categoryID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllByStatusAndCategoryAsync(ArticleStatus.Pending, categoryID)).ReturnsAsync((IEnumerable<Article>)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _articleService.GetPaginatedArticlesAsync(1, 1, status, categoryID));
        }

        [Test]
        public async Task GetPaginatedArticlesForUserAsync_ShouldReturnPaginatedArticles_WhenArticlesExist()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article { ArticleID = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Article { ArticleID = 2, CreatedAt = DateTime.UtcNow }
            };
            var categoryID = 1;
            var userID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllApprcvedEditedArticlesAndCategoryAsync(categoryID)).ReturnsAsync(articles);
            _savedArticleServiceMock.Setup(service => service.CheckForSaved(It.IsAny<int>(), userID)).ReturnsAsync(true);

            // Act
            var result = await _articleService.GetPaginatedArticlesForUserAsync(1, 1, categoryID, userID);

            // Assert
            Assert.AreEqual(1, result.Articles.Count());
            Assert.AreEqual(2, result.totalpages);
            Assert.IsTrue(result.Articles.First().isSaved);
        }

        [Test]
        public void GetPaginatedArticlesForUserAsync_ShouldThrowNoAvailableItemException_WhenNoArticlesExist()
        {
            // Arrange
            var categoryID = 1;
            var userID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllApprcvedEditedArticlesAndCategoryAsync(categoryID)).ReturnsAsync((IEnumerable<Article>)null);

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(() => _articleService.GetPaginatedArticlesForUserAsync(1, 1, categoryID, userID));
        }

        [Test]
        public async Task GetPaginatedFeedsForUserAsync_ShouldReturnFilteredPaginatedArticles_WhenPreferencesExist()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article { ArticleID = 1, CreatedAt = DateTime.UtcNow.AddDays(-1), ArticleCategories = new List<ArticleCategory>{ new ArticleCategory{ CategoryID = 1} } },
                new Article { ArticleID = 2, CreatedAt = DateTime.UtcNow, ArticleCategories = new List<ArticleCategory>{ new ArticleCategory{ CategoryID = 2} } }
            };
            var userPreferences = new List<UserPreference>
            {
                new UserPreference { UserID = 1, CategoryID = 1, preference = Preference.Like }
            };
            var userID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllApprcvedEditedArticlesAsync()).ReturnsAsync(articles);
            _userPreferenceRepositoryMock.Setup(repo => repo.GetAll("UserID", userID.ToString())).ReturnsAsync(userPreferences);
            _savedArticleServiceMock.Setup(service => service.CheckForSaved(It.IsAny<int>(), userID)).ReturnsAsync(true);

            // Act
            var result = await _articleService.GetPaginatedFeedsForUserAsync(1, 1, userID);

            // Assert
            Assert.AreEqual(1, result.Articles.Count());
            Assert.AreEqual(1, result.totalpages);
            Assert.IsTrue(result.Articles.First().isSaved);
        }

        [Test]
        public async Task GetPaginatedFeedsForUserAsync_ShouldReturnFilteredPaginatedArticles_WhenPreferencesNOtExist()
        {
            // Arrange
            var articles = new List<Article>
            {
                new Article { ArticleID = 1, CreatedAt = DateTime.UtcNow.AddDays(-1), ArticleCategories = new List<ArticleCategory>{ new ArticleCategory{ CategoryID = 1} } },
                new Article { ArticleID = 2, CreatedAt = DateTime.UtcNow, ArticleCategories = new List<ArticleCategory>{ new ArticleCategory{ CategoryID = 2} } }
            };
            var userPreferences = new List<UserPreference>
            {
               
            };
            var userID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllApprcvedEditedArticlesAsync()).ReturnsAsync(articles);
            _userPreferenceRepositoryMock.Setup(repo => repo.GetAll("UserID", userID.ToString())).ReturnsAsync(userPreferences);
            _savedArticleServiceMock.Setup(service => service.CheckForSaved(It.IsAny<int>(), userID)).ReturnsAsync(true);

            // Act
            var result = await _articleService.GetPaginatedFeedsForUserAsync(1, 2, userID);

            // Assert
            Assert.AreEqual(2, result.Articles.Count());
            Assert.AreEqual(1, result.totalpages);
            Assert.IsTrue(result.Articles.First().isSaved);
        }

        [Test]
        public void GetPaginatedFeedsForUserAsync_ShouldThrowNoAvailableItemException_WhenNoArticlesExist()
        {
            // Arrange
            var userID = 1;

            _articleRepositoryMock.Setup(repo => repo.GetAllApprcvedEditedArticlesAsync()).ReturnsAsync((IEnumerable<Article>)null);

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(() => _articleService.GetPaginatedFeedsForUserAsync(1, 1, userID));
        }

        [Test]
        public async Task EditArticleData_ShouldEditArticle_WhenArticleExists()
        {
            // Arrange
            var adminArticleEditDTO = new AdminArticleEditGetDTO
            {
                ArticleID = 123,
                Title = "New Title",
                Content = "New Content",
                ImgURL = "http://newimage.url",
                Summary = "New Summary",
                OriginURL = "http://neworigin.url",
                Categories = new List<int> { 1, 2 }
            };
            var article = new Article { ArticleID = 123 };
            var updatedArticle = new Article { ArticleID = 123, Title = "New Title", Content = "New Content" };

            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", adminArticleEditDTO.ArticleID.ToString()))
                .ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString()))
                .ReturnsAsync(updatedArticle);
            _articleCategoryRepositoryMock.Setup(repo => repo.DeleteByArticleID(article.ArticleID.ToString()))
                .ReturnsAsync(new List<ArticleCategory>());
            _articleCategoryRepositoryMock.Setup(repo => repo.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((ArticleCategory)null);
            _articleCategoryRepositoryMock.Setup(repo => repo.Add(It.IsAny<ArticleCategory>()))
                .ReturnsAsync(new ArticleCategory { ArticleID = article.ArticleID });

            // Act
            var result = await _articleService.EditArticleData(adminArticleEditDTO);

            // Assert
            Assert.AreEqual("New Title", result.Title);
            _articleRepositoryMock.Verify(repo => repo.Get("ArticleID", adminArticleEditDTO.ArticleID.ToString()), Times.Once);
            _articleRepositoryMock.Verify(repo => repo.Update(article, article.ArticleID.ToString()), Times.Once);
            _articleCategoryRepositoryMock.Verify(repo => repo.DeleteByArticleID(article.ArticleID.ToString()), Times.Once);
            _articleCategoryRepositoryMock.Verify(repo => repo.Add(It.IsAny<ArticleCategory>()), Times.Exactly(2));
        }

        [Test]
        public async Task EditArticleData_ShouldEditArticle_WhenUpdateError()
        {
            // Arrange
            var adminArticleEditDTO = new AdminArticleEditGetDTO
            {
                ArticleID = 123,
                Title = "New Title",
                Content = "New Content",
                ImgURL = "http://newimage.url",
                Summary = "New Summary",
                OriginURL = "http://neworigin.url",
                Categories = new List<int> { 1, 2 }
            };
            var article = new Article { ArticleID = 123 };
            var updatedArticle = new Article { ArticleID = 123, Title = "New Title", Content = "New Content" };

            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", adminArticleEditDTO.ArticleID.ToString()))
                .ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString()))
                .ReturnsAsync(updatedArticle);
            _articleCategoryRepositoryMock.Setup(repo => repo.DeleteByArticleID(article.ArticleID.ToString()))
                .ReturnsAsync(new List<ArticleCategory>());
            _articleCategoryRepositoryMock.Setup(repo => repo.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((ArticleCategory)null);
            _articleCategoryRepositoryMock.Setup(repo => repo.Add(It.IsAny<ArticleCategory>()))
                .ReturnsAsync(new ArticleCategory());

 

            // Assert
            Assert.ThrowsAsync<UnableToUpdateItemException>(() => _articleService.EditArticleData(adminArticleEditDTO));
        }

        [Test]
        public void EditArticleData_ShouldThrowItemNotFoundException_WhenArticleDoesNotExist()
        {
            // Arrange
            var adminArticleEditDTO = new AdminArticleEditGetDTO { ArticleID = 123 };
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", adminArticleEditDTO.ArticleID.ToString()))
                .ReturnsAsync((Article)null);

            // Act & Assert
            Assert.ThrowsAsync<ItemNotFoundException>(() => _articleService.EditArticleData(adminArticleEditDTO));
        }

        [Test]
        public async Task UpdateShareCount_ShouldUpdateShareCount_WhenShareDataIsValid()
        {
            // Arrange
            var shareDataDTO = new ShareDataDTO { ArticleID = 123, UserID = 1, Platform = "Facebook" };
            var article = new Article { ArticleID = 123, ShareCount = 0 };
            var shareData = new ShareData { ArticleID = 123, UserID = 1, Platform = "Facebook" };

            _shareDataRepositoryMock.Setup(repo => repo.Add(It.IsAny<ShareData>()))
                .ReturnsAsync(shareData);
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", shareDataDTO.ArticleID.ToString()))
                .ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString()))
                .ReturnsAsync(article);

            var mockClientProxy = new Mock<IClientProxy>();
            _hubContextMock.Setup(hub => hub.Clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);
            mockClientProxy.Setup(client => client.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _articleService.UpdateShareCount(shareDataDTO);

            // Assert
            Assert.AreEqual(1, article.ShareCount);
            Assert.AreEqual(shareData.ArticleID, result.ArticleID);
            _shareDataRepositoryMock.Verify(repo => repo.Add(It.IsAny<ShareData>()), Times.Once);
            _articleRepositoryMock.Verify(repo => repo.Get("ArticleID", shareDataDTO.ArticleID.ToString()), Times.Once);
            _articleRepositoryMock.Verify(repo => repo.Update(article, article.ArticleID.ToString()), Times.Once);
            mockClientProxy.Verify(client => client.SendCoreAsync("UpdateShareCount",
                It.Is<object[]>(args => args[0].ToString() == article.ArticleID.ToString() && (int)args[1] == article.ShareCount), default), Times.Once);
        }

        [Test]
        public void UpdateShareCount_ShouldThrowUnableToAddItemException_WhenShareDataIsNotAdded()
        {
            // Arrange
            var shareDataDTO = new ShareDataDTO { ArticleID = 123, UserID = 1, Platform = "Facebook" };
            _shareDataRepositoryMock.Setup(repo => repo.Add(It.IsAny<ShareData>()))
                .ReturnsAsync((ShareData)null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToAddItemException>(() => _articleService.UpdateShareCount(shareDataDTO));
        }

        [Test]
        public void UpdateShareCount_ShouldThrowUnableToUpdateItemException_WhenArticleIsNotUpdated()
        {
            // Arrange
            var shareDataDTO = new ShareDataDTO { ArticleID = 123, UserID = 1, Platform = "Facebook" };
            var shareData = new ShareData { ArticleID = 123, UserID = 1, Platform = "Facebook" };
            var article = new Article { ArticleID = 123, ShareCount = 0 };

            _shareDataRepositoryMock.Setup(repo => repo.Add(It.IsAny<ShareData>()))
                .ReturnsAsync(shareData);
            _articleRepositoryMock.Setup(repo => repo.Get("ArticleID", shareDataDTO.ArticleID.ToString()))
                .ReturnsAsync(article);
            _articleRepositoryMock.Setup(repo => repo.Update(article, article.ArticleID.ToString()))
                .ReturnsAsync((Article)null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToUpdateItemException>(() => _articleService.UpdateShareCount(shareDataDTO));
        }
    }
}
