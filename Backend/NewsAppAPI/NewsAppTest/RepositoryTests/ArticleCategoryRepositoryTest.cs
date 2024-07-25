using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Classes;


namespace NewsAppTest.RepositoryTests
{
    public class ArticleCategoryRepositoryTest
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
        public async Task Add_ArticleCategory_Success()
        {
            var context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };

            var result = await _articleCategoryRepository.Add(articleCategory1);

            Assert.NotNull(result);
            Assert.AreEqual(articleCategory1.CategoryID, result.CategoryID);
        }

        [Test]
        public async Task Delete_ArticleCategory_ByComposite_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };

            await _articleCategoryRepository.Add(articleCategory1);
            var articltodelete = _context.ArticleCategories.First();

            var result = await _articleCategoryRepository.Delete(articltodelete.CategoryID.ToString()+"-"+ articltodelete.ArticleID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(articltodelete.CategoryID, result.CategoryID);

            var userInDb = await _context.Users.FindAsync(articltodelete.CategoryID);
            Assert.Null(userInDb);
        }

        [Test]
        public async Task Delete_ArticleCategory_ByComposite_NotFoundException()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _articleCategoryRepository.Delete("1-5"));
        }

        [Test]
        public async Task Delete_ArticleCategory_ByComposite_ArgumentException()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleCategoryRepository.Delete("1-5-3"));
        }

        [Test]
        public async Task Delete_ArticleCategory_ByComposite_ArgumentException1()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleCategoryRepository.Delete("1"));
        }

        [Test]
        public async Task Delete_ArticleCategory_ByCategoryId_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };

            await _articleCategoryRepository.Add(articleCategory1);
            var articltodelete = _context.ArticleCategories.First();

            var result = await _articleCategoryRepository.DeleteByCategoryID(articltodelete.CategoryID.ToString());

            Assert.NotNull(result);

            var userInDb = await _context.Users.FindAsync(articltodelete.CategoryID);
            Assert.Null(userInDb);
        }

        [Test]
        public async Task Delete_ArticleCategory_ByCategoryId_Exception()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _articleCategoryRepository.DeleteByCategoryID("1"));
        }

        [Test]
        public async Task Delete_ArticleCategory_ByArticlId_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };

            await _articleCategoryRepository.Add(articleCategory1);
            var articltodelete = _context.ArticleCategories.First();

            var result = await _articleCategoryRepository.DeleteByArticleID(articltodelete.CategoryID.ToString());

            Assert.NotNull(result);

            var userInDb = await _context.Users.FindAsync(articltodelete.CategoryID);
            Assert.Null(userInDb);
        }

        [Test]
        public async Task Delete_ArticleCategory_ByArticleId_Exception()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _articleCategoryRepository.DeleteByArticleID("1"));
        }


        [Test]
        public async Task Get_AllArticleCategory_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };

            await _articleCategoryRepository.Add(articleCategory1);
            var result = await _articleCategoryRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task Get_AllArticleCategory_NoAvailableItemException()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);
            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _articleCategoryRepository.GetAll("", ""));
        }

        [Test]
        public async Task Get_AllByColumn_Success()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };

            var articleCategory2 = new ArticleCategory
            {
                CategoryID = 2,
                ArticleID = 1,
            };

            var articleCategory3 = new ArticleCategory
            {
                CategoryID = 3,
                ArticleID = 1,
            };


            await _articleCategoryRepository.Add(articleCategory1);
            await _articleCategoryRepository.Add(articleCategory2);
            await _articleCategoryRepository.Add(articleCategory3);
            var result = await _articleCategoryRepository.GetAll("ArticleID", "1");

            var result1 = await _articleCategoryRepository.GetAll("CategoryID", "3");

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());

            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());
        }


        [Test]
        public async Task Get_AllByColumn_Exception()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _articleCategoryRepository.GetAll("ArticleID", "2"));
        }

        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _articleCategoryRepository = new ArticleCategoryRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _articleCategoryRepository.GetAll("Title", "3"));
        }

        [Test]
        public async Task GetArticleCategoryById_ShouldReturnArticleCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ArticleCategoryRepository(context);
            var articleCategory1 = new ArticleCategory
            {
                CategoryID = 1,
                ArticleID = 1,
            };
            context.ArticleCategories.Add(articleCategory1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("CategoryID", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.CategoryID, Is.EqualTo(articleCategory1.CategoryID));
        }

        [Test]
        public void GetUserById_ShouldThrowNotFoundException()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ArticleCategoryRepository(context);

            // Act & Assert
            Assert.ThrowsAsync<ItemNotFoundException>(async () => await repository.Get("CategoryID", "5"));
        }
    }
}
