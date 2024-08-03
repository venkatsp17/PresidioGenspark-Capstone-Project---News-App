using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAppTest.RepositoryTests
{
    public class ShareDataRepositoryTest
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
        public async Task Add_ShareData_Success()
        {
            var context = GetInMemoryDbContext();
            var _shareDataRepository = new ShareDataRepository(context);
            var shareData = new ShareData
            {
               ArticleID=1,
               UserID=1,
               Platform="Facebook"
            };

            var result = await _shareDataRepository.Add(shareData);

            Assert.NotNull(result);
            Assert.AreEqual(shareData.Id, result.Id);
        }

        [Test]
        public async Task Delete_ShareData_Success()
        {
            var _context = GetInMemoryDbContext();
            var _shareDataRepository = new ShareDataRepository(_context);
            var shareData1 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "Facebook"
            };

            await _shareDataRepository.Add(shareData1);
            var sharedatatodelete = _context.ShareDatas.First();

            var result = await _shareDataRepository.Delete(sharedatatodelete.Id.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(sharedatatodelete.Id, result.Id);

            var userInDb = await _context.Users.FindAsync(sharedatatodelete.Id);
            Assert.Null(userInDb);
        }


        [Test]
        public async Task Update_ShareData_Success()
        {
            var _context = GetInMemoryDbContext();
            var _shareDataRepository = new ShareDataRepository(_context);
            var shareData1 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "Facebook"
            };

            await _shareDataRepository.Add(shareData1);
            var categoryToUpdate = _context.ShareDatas.First();
            categoryToUpdate.Platform = "X";

            var result = await _shareDataRepository.Update(categoryToUpdate, categoryToUpdate.Id.ToString());

            Assert.NotNull(result);
            Assert.AreEqual("X", result.Platform);

            var categoryInDb = await _context.ShareDatas.FindAsync(categoryToUpdate.Id);
            Assert.NotNull(categoryInDb);
            Assert.AreEqual("X", categoryInDb.Platform);
        }


        [Test]
        public async Task Get_AllShareData_Success()
        {
            var _context = GetInMemoryDbContext();
            var _shareDataRepository = new ShareDataRepository(_context);
            var shareData1 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "Facebook"
            };

            await _shareDataRepository.Add(shareData1);
            var result = await _shareDataRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }


        [Test]
        public async Task Get_AllByColumn_Success()
        {
            var _context = GetInMemoryDbContext();
            var _shareDataRepository = new ShareDataRepository(_context);
            var shareData1 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "Facebook"
            };

            var shareData2 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "X"
            };

            var shareData3 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "Whatsapp"
            };


            await _shareDataRepository.Add(shareData1);
            await _shareDataRepository.Add(shareData2);
            await _shareDataRepository.Add(shareData3);
            var result = await _shareDataRepository.GetAll("Platform", "X");

            var result1 = await _shareDataRepository.GetAll("Id", "2");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());

            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());
        }


        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _shareDataRepository = new ShareDataRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _shareDataRepository.GetAll("Title", "Economy"));
        }

        [Test]
        public async Task GetShareDataById_ShouldReturnShareData()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ShareDataRepository(context);
            var shareData1 = new ShareData
            {
                ArticleID = 1,
                UserID = 1,
                Platform = "Facebook"
            };
            context.ShareDatas.Add(shareData1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("Id", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Id, Is.EqualTo(shareData1.Id));
        }
    }
}
