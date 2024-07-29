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
    public class SavedArticleRepositoryTest
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
        public async Task Add_SavedArticle_Success()
        {
            var context = GetInMemoryDbContext();
            var _savedArticleRepository = new SavedArticleRepository(context);
            var savedArticle1 = new SavedArticle
            {
                SavedArticleID = 1,
                ArticleID = 1,
                UserID = 1,                
                SavedAt = DateTime.UtcNow
            };

            var result = await _savedArticleRepository.Add(savedArticle1);

            Assert.NotNull(result);
            Assert.AreEqual(savedArticle1.SavedArticleID, result.SavedArticleID);
        }

        [Test]
        public async Task Delete_SavedArticle_Success()
        {
            var _context = GetInMemoryDbContext();
            var _savedArticleRepository = new SavedArticleRepository(_context);
            var savedArticle1 = new SavedArticle
            {
                SavedArticleID = 1,
                ArticleID = 1,
                UserID = 1,                
                SavedAt = DateTime.UtcNow
            };

            await _savedArticleRepository.Add(savedArticle1);
            var articltodelete = _context.SavedArticles.First();

            var result = await _savedArticleRepository.Delete(articltodelete.SavedArticleID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(articltodelete.SavedArticleID, result.SavedArticleID);

            var userInDb = await _context.Users.FindAsync(articltodelete.SavedArticleID);
            Assert.Null(userInDb);
        }

  

        [Test]
        public async Task Update_SavedArticle_Success()
        {
            var _context = GetInMemoryDbContext();
            var _savedArticleRepository = new SavedArticleRepository(_context);
            var savedArticle1 = new SavedArticle
            {
                SavedArticleID = 1,
                ArticleID = 1,
                UserID = 1,                
                SavedAt = DateTime.UtcNow
            };

            await _savedArticleRepository.Add(savedArticle1);
            var savedArticleToUpdate = _context.SavedArticles.First();
            savedArticleToUpdate.ArticleID = 2;

            var result = await _savedArticleRepository.Update(savedArticleToUpdate, savedArticleToUpdate.SavedArticleID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(2, result.ArticleID);

            var savedArticleInDb = await _context.SavedArticles.FindAsync(savedArticleToUpdate.SavedArticleID);
            Assert.NotNull(savedArticleInDb);
            Assert.AreEqual(2, savedArticleInDb.ArticleID);
        }

      

        [Test]
        public async Task Get_AllSavedArticle_Success()
        {
            var _context = GetInMemoryDbContext();
            var _savedArticleRepository = new SavedArticleRepository(_context);
            var savedArticle1 = new SavedArticle
            {
                SavedArticleID = 1,
                ArticleID = 1,
                UserID = 1,                
                SavedAt = DateTime.UtcNow
            };

            await _savedArticleRepository.Add(savedArticle1);
            var result = await _savedArticleRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }

     

        [Test]
        public async Task Get_AllByColumn_Success()
        {
            var _context = GetInMemoryDbContext();
            var _savedArticleRepository = new SavedArticleRepository(_context);
            var savedArticle1 = new SavedArticle
            {
                SavedArticleID = 1,
                ArticleID = 1,
                UserID = 1,                
                SavedAt = DateTime.UtcNow
            };

            var savedArticle2 = new SavedArticle
            {
                SavedArticleID = 2,
                ArticleID = 1,
                UserID = 2,                
                SavedAt = DateTime.UtcNow
            };

            var savedArticle3 = new SavedArticle
            {
                SavedArticleID = 3,
                ArticleID = 1,
                UserID = 3,               
                SavedAt = DateTime.UtcNow
            };


            await _savedArticleRepository.Add(savedArticle1);
            await _savedArticleRepository.Add(savedArticle2);
            await _savedArticleRepository.Add(savedArticle3);
            var result = await _savedArticleRepository.GetAll("ArticleID", "1");

            var result1 = await _savedArticleRepository.GetAll("SavedArticleID", "3");

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());

            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());
        }


    

        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _savedArticleRepository = new SavedArticleRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _savedArticleRepository.GetAll("Title", "3"));
        }

        [Test]
        public async Task GetSavedArticleById_ShouldReturnSavedArticle()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new SavedArticleRepository(context);
            var savedArticle1 = new SavedArticle
            {
                SavedArticleID = 1,
                ArticleID = 1,
                UserID = 1,                
                SavedAt = DateTime.UtcNow
            };
            context.SavedArticles.Add(savedArticle1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("SavedArticleID", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.SavedArticleID, Is.EqualTo(savedArticle1.SavedArticleID));
        }

    
    }
}
