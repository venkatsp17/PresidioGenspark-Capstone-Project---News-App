using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Classes;
using static NewsApp.Models.Enum;

namespace NewsAppTest.RepositoryTests
{
    public class ArticleRepositoryTest
    {

        private NewsAppDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<NewsAppDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new NewsAppDBContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Test]
        public async Task Add_Article_Success()
        {
            var context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(context);
            var article1 = new Article
            {
                ArticleID=1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID="",
                OldHashID=""
            };

            var result = await _articleRepository.Add(article1);

            Assert.NotNull(result);
            Assert.AreEqual(article1.ArticleID, result.ArticleID);
        }

        [Test]
        public async Task Delete_Article_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = ""
            };

            await _articleRepository.Add(article1);
            var articltodelete = _context.Articles.First();

            var result = await _articleRepository.Delete(articltodelete.ArticleID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(articltodelete.ArticleID, result.ArticleID);

            var userInDb = await _context.Users.FindAsync(articltodelete.ArticleID);
            Assert.Null(userInDb);
        }


        [Test]
        public async Task Update_Article_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = ""
            };

            await _articleRepository.Add(article1);
            var articleToUpdate = _context.Articles.First();
            articleToUpdate.Title = "Breaking News: Tech Destruction";

            var result = await _articleRepository.Update(articleToUpdate, articleToUpdate.ArticleID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual("Breaking News: Tech Destruction", result.Title);

            var articleInDb = await _context.Articles.FindAsync(articleToUpdate.ArticleID);
            Assert.NotNull(articleInDb);
            Assert.AreEqual("Breaking News: Tech Destruction", articleInDb.Title);
        }


        [Test]
        public async Task Get_AllArticle_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = ""
            };

            await _articleRepository.Add(article1);
            var result = await _articleRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }


        [Test]
        public async Task Get_AllByColumnStatus_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = "",
                Status= ArticleStatus.Approved
            };

            var article2 = new Article
            {
                ArticleID = 2,
                Title = "Health Economy Update",
                Content = "Insights and updates on the global economy...",
                Summary = "A summary of the global economy article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/global-economy",
                ImgURL = "https://example.com/images/economy.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 8.2m,
                SaveCount = 150,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Approved
            };

            var article3 = new Article
            {
                ArticleID = 3,
                Title = "Health and Wellness Tips",
                Content = "Comprehensive guide to maintaining health and wellness...",
                Summary = "A brief summary of health and wellness tips.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/health-wellness",
                ImgURL = "https://example.com/images/health.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.0m,
                SaveCount = 200,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Rejected
            };


            await _articleRepository.Add(article1);
            await _articleRepository.Add(article2);
            await _articleRepository.Add(article3);
            var result = await _articleRepository.GetAll("Title", "Health");

            var result1 = await _articleRepository.GetAll("ArticleID", "3");

            var result2 = await _articleRepository.GetAll("Status", "2");

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());

            Assert.NotNull(result2);
            Assert.AreEqual(2, result2.Count());


            Assert.ThrowsAsync<ArgumentException>(async () => await _articleRepository.GetAll("Status", "1fds"));
        }


        [Test]
        public async Task GetAllApprcvedEditedArticlesAndCategoryAsyncSuccess()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Approved,
                 ArticleCategories = new List<ArticleCategory>
            {
                new ArticleCategory { CategoryID = 1 }
            }
            };

            var article2 = new Article
            {
                ArticleID = 2,
                Title = "Health Economy Update",
                Content = "Insights and updates on the global economy...",
                Summary = "A summary of the global economy article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/global-economy",
                ImgURL = "https://example.com/images/economy.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 8.2m,
                SaveCount = 150,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Approved,
                ArticleCategories = new List<ArticleCategory>
            {
                new ArticleCategory { CategoryID = 1 }
            }
            };

            var article3 = new Article
            {
                ArticleID = 3,
                Title = "Health and Wellness Tips",
                Content = "Comprehensive guide to maintaining health and wellness...",
                Summary = "A brief summary of health and wellness tips.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/health-wellness",
                ImgURL = "https://example.com/images/health.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.0m,
                SaveCount = 200,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Rejected,
                ArticleCategories = new List<ArticleCategory>
            {
                new ArticleCategory { CategoryID = 2 }
            }
            };


            await _articleRepository.Add(article1);
            await _articleRepository.Add(article2);
            await _articleRepository.Add(article3);
            var result = await _articleRepository.GetAllApprcvedEditedArticlesAndCategoryAsync(1);


            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());



            Assert.ThrowsAsync<ArgumentException>(async () => await _articleRepository.GetAll("Status", "1fds"));
        }


        [Test]
        public async Task GetAllByStatusAndCategoryAsyncSuccess()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Approved,
                ArticleCategories = new List<ArticleCategory>
            {
                new ArticleCategory { CategoryID = 1 }
            }
            };

            var article2 = new Article
            {
                ArticleID = 2,
                Title = "Health Economy Update",
                Content = "Insights and updates on the global economy...",
                Summary = "A summary of the global economy article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/global-economy",
                ImgURL = "https://example.com/images/economy.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 8.2m,
                SaveCount = 150,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Approved,
                ArticleCategories = new List<ArticleCategory>
            {
                new ArticleCategory { CategoryID = 1 }
            }
            };

            var article3 = new Article
            {
                ArticleID = 3,
                Title = "Health and Wellness Tips",
                Content = "Comprehensive guide to maintaining health and wellness...",
                Summary = "A brief summary of health and wellness tips.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/health-wellness",
                ImgURL = "https://example.com/images/health.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.0m,
                SaveCount = 200,
                HashID = "",
                OldHashID = "",
                Status = ArticleStatus.Rejected,
                ArticleCategories = new List<ArticleCategory>
            {
                new ArticleCategory { CategoryID = 2 }
            }
            };


            await _articleRepository.Add(article1);
            await _articleRepository.Add(article2);
            await _articleRepository.Add(article3);
            var result = await _articleRepository.GetAllByStatusAndCategoryAsync(ArticleStatus.Approved,1);


            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());



            Assert.ThrowsAsync<ArgumentException>(async () => await _articleRepository.GetAll("Status", "1fds"));
        }

        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _articleRepository = new ArticleRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _articleRepository.GetAll("Heading", "Economy"));
        }

        [Test]
        public async Task GetArticleById_ShouldReturnArticle()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ArticleRepository(context);
            var article1 = new Article
            {
                ArticleID = 1,
                Title = "Breaking News: Tech Innovation",
                Content = "Detailed content about the latest in tech innovation...",
                Summary = "A brief summary of the tech innovation article.",
                AddedAt = DateTime.UtcNow,
                OriginURL = "https://example.com/tech-innovation",
                ImgURL = "https://example.com/images/tech.jpg",
                CreatedAt = DateTime.UtcNow,
                ImpScore = 9.5m,
                SaveCount = 100,
                HashID = "",
                OldHashID = ""
            };
            context.Articles.Add(article1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("ArticleID", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.ArticleID, Is.EqualTo(article1.ArticleID));
        }

    }
}
