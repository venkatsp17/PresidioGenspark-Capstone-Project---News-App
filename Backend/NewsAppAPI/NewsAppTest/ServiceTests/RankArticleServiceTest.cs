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
    [TestFixture]
    public class RankArticleServiceTest
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<ISavedArticleService> _savedArticleServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IUserPreferenceRepository> _userPreferenceRepositoryMock;
        private RankArticleService _rankArticleService;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _savedArticleServiceMock = new Mock<ISavedArticleService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userPreferenceRepositoryMock = new Mock<IUserPreferenceRepository>();
            _rankArticleService = new RankArticleService(
                _articleRepositoryMock.Object,
                _savedArticleServiceMock.Object,
                _userRepositoryMock.Object,
                _userPreferenceRepositoryMock.Object
            );
        }

        // Test for GetAllStatistics method
        [Test]
        public async Task GetAllStatistics_ShouldReturnStatisticsDto_WhenAllDataIsValid()
        {
            // Arrange
            var totalUserCount = 100;
            var totalApprovedArticleCount = 50;
            var totalEditedArticleCount = 20;
            var totalRejectedArticleCount = 10;
            var totalPendingArticleCount = 15;
            var mostCommentedArticle = new ArticleDto { Id = 1, CommentCount = 100 };
            var mostSavedArticle = new ArticleDto { Id = 2, SaveCount = 50 };
            var mostSharedArticle = new ArticleDto { Id = 3, ShareCount = 75 };
            var categoryPreferences = new List<CategoryPreferenceDto>();

            _userRepositoryMock.Setup(repo => repo.GetAllUserCountAsync()).ReturnsAsync(totalUserCount);
            _articleRepositoryMock.Setup(repo => repo.GetAllArticleCountByStatus(ArticleStatus.Approved)).ReturnsAsync(totalApprovedArticleCount);
            _articleRepositoryMock.Setup(repo => repo.GetAllArticleCountByStatus(ArticleStatus.Edited)).ReturnsAsync(totalEditedArticleCount);
            _articleRepositoryMock.Setup(repo => repo.GetAllArticleCountByStatus(ArticleStatus.Rejected)).ReturnsAsync(totalRejectedArticleCount);
            _articleRepositoryMock.Setup(repo => repo.GetAllArticleCountByStatus(ArticleStatus.Pending)).ReturnsAsync(totalPendingArticleCount);
            _articleRepositoryMock.Setup(repo => repo.MostInteractedArticle("comment")).ReturnsAsync(mostCommentedArticle);
            _articleRepositoryMock.Setup(repo => repo.MostInteractedArticle("saved")).ReturnsAsync(mostSavedArticle);
            _articleRepositoryMock.Setup(repo => repo.MostInteractedArticle("shared")).ReturnsAsync(mostSharedArticle);
            _userPreferenceRepositoryMock.Setup(repo => repo.LikedDiskedAriclesORder()).ReturnsAsync(categoryPreferences);

            // Act
            var result = await _rankArticleService.GetAllStatistics();

            // Assert
            Assert.AreEqual(totalUserCount, result.TotalUserCount);
            Assert.AreEqual(totalApprovedArticleCount, result.TotalApprovedArticleCount);
            Assert.AreEqual(totalEditedArticleCount, result.TotalEditedArticleCount);
            Assert.AreEqual(totalRejectedArticleCount, result.TotalRejectedArticleCount);
            Assert.AreEqual(totalPendingArticleCount, result.TotalPendingArticleCount);
            Assert.AreEqual(mostCommentedArticle, result.MostCommentedArticle);
            Assert.AreEqual(mostSavedArticle, result.MostSavedArticle);
            Assert.AreEqual(mostSharedArticle, result.MostSharedArticle);
            Assert.AreEqual(categoryPreferences, result.CategoryPreferences);
        }

        // Test for RankTop3Articles method
        [Test]
        public async Task RankTop3Articles_ShouldReturnTop3RankedArticles_WhenArticlesAreAvailable()
        {
            // Arrange
            var category = 1;
            var userId = 1;

            var articles = new List<Article>
            {
                new Article { ArticleID = 1, CommentCount = 10, SaveCount = 5, ShareCount = 2 },
                new Article { ArticleID = 2, CommentCount = 20, SaveCount = 15, ShareCount = 10 },
                new Article { ArticleID = 3, CommentCount = 5, SaveCount = 25, ShareCount = 30 },
                new Article { ArticleID = 4, CommentCount = 30, SaveCount = 10, ShareCount = 20 }
            };

            _articleRepositoryMock.Setup(repo => repo.GetArticlesForRanking(category)).ReturnsAsync(articles);
            _savedArticleServiceMock.Setup(service => service.CheckForSaved(It.IsAny<int>(), userId)).ReturnsAsync(true);

            // Act
            var result = await _rankArticleService.RankTop3Articles(category, userId);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(3, result.First().ArticleID);
            Assert.AreEqual(2, result.Last().ArticleID);
            _savedArticleServiceMock.Verify(service => service.CheckForSaved(It.IsAny<int>(), userId), Times.Exactly(3));
        }

        // Test for RankTop3Articles method when no articles are available
        [Test]
        public void RankTop3Articles_ShouldThrowNoAvailableItemException_WhenNoArticlesAreAvailable()
        {
            // Arrange
            var category = 1;
            var userId = 1;

            _articleRepositoryMock.Setup(repo => repo.GetArticlesForRanking(category)).ReturnsAsync(new List<Article>());

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _rankArticleService.RankTop3Articles(category, userId));
        }
    }
}
