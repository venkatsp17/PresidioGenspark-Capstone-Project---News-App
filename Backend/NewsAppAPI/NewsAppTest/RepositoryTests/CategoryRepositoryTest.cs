using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories;
using NewsApp.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAppTest.RepositoryTests
{
    public class CategoryRepositoryTest
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
        public async Task Add_Category_Success()
        {
            var context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Technology",
                Description = "Articles related to the latest technology trends and updates."
            };

            var result = await _categoryRepository.Add(category1);

            Assert.NotNull(result);
            Assert.AreEqual(category1.CategoryID, result.CategoryID);
        }

        [Test]
        public async Task Delete_Category_Success()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Technology",
                Description = "Articles related to the latest technology trends and updates."
            };

            await _categoryRepository.Add(category1);
            var articltodelete = _context.Categories.First();

            var result = await _categoryRepository.Delete(articltodelete.CategoryID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(articltodelete.CategoryID, result.CategoryID);

            var userInDb = await _context.Users.FindAsync(articltodelete.CategoryID);
            Assert.Null(userInDb);
        }

        [Test]
        public async Task Delete_Category_NotFoundException()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);

            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _categoryRepository.Delete("5"));
        }

        [Test]
        public async Task Update_Category_Success()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Technology",
                Description = "Articles related to the latest technology trends and updates."
            };

            await _categoryRepository.Add(category1);
            var categoryToUpdate = _context.Categories.First();
            categoryToUpdate.Name = "Tech";

            var result = await _categoryRepository.Update(categoryToUpdate, categoryToUpdate.CategoryID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual("Tech", result.Name);

            var categoryInDb = await _context.Categories.FindAsync(categoryToUpdate.CategoryID);
            Assert.NotNull(categoryInDb);
            Assert.AreEqual("Tech", categoryInDb.Name);
        }

        [Test]
        public async Task Update_Category_NotFoundException()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Technology",
                Description = "Articles related to the latest technology trends and updates."
            };
            category1.Name = "Tech";

            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _categoryRepository.Update(category1, category1.CategoryID.ToString()));
        }

        [Test]
        public async Task Get_AllCategory_Success()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Technology",
                Description = "Articles related to the latest technology trends and updates."
            };

            await _categoryRepository.Add(category1);
            var result = await _categoryRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task Get_AllCategory_NoAvailableItemException()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);

            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _categoryRepository.GetAll("", ""));
        }

        [Test]
        public async Task Get_AllByColumn_Success()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Technology",
                Description = "Articles related to the latest technology trends and updates."
            };

            var category2 = new Category
            {
                CategoryID = 2,
                Name = "Health",
                Description = "Insights and news on health and wellness topics."
            };

            var category3 = new Category
            {
                CategoryID = 3,
                Name = "Entertainment",
                Description = "Latest updates and news from the entertainment industry."
            };


            await _categoryRepository.Add(category1);
            await _categoryRepository.Add(category2);
            await _categoryRepository.Add(category3);
            var result = await _categoryRepository.GetAll("Name", "enterta");

            var result1 = await _categoryRepository.GetAll("CategoryID", "3");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());

            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());
        }


        [Test]
        public async Task Get_AllByColumn_Exception()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);

            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _categoryRepository.GetAll("Name", "Economy"));
        }

        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _categoryRepository = new CategoryRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _categoryRepository.GetAll("Title", "Economy"));
        }

        [Test]
        public async Task GetCategoryById_ShouldReturnCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);
            var category1 = new Category
            {
                CategoryID = 1,
                Name = "Entertainment",
                Description = "Latest updates and news from the entertainment industry."
            };
            context.Categories.Add(category1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("CategoryID", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.CategoryID, Is.EqualTo(category1.CategoryID));
        }

        [Test]
        public void GetUserById_ShouldThrowNotFoundException()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            // Act & Assert
            Assert.ThrowsAsync<ItemNotFoundException>(async () => await repository.Get("CategoryID", "5"));
        }
    }
}
