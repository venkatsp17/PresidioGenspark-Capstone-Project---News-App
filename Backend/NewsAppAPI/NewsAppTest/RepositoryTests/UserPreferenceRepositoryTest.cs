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
using static NewsApp.Models.Enum;

namespace NewsAppTest.RepositoryTests
{
    public class UserPreferenceRepositoryTest
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
        public async Task Add_UserPreference_Success()
        {
            var context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(context);
            var preferenceData = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference=0
            };

            var result = await _userPreferenceRepository.Add(preferenceData);

            Assert.NotNull(result);
            Assert.AreEqual(preferenceData.UserPreferenceID, result.UserPreferenceID);
        }

        [Test]
        public async Task Delete_UserPreference_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(_context);
            var preferenceData1 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference=0
            };

            await _userPreferenceRepository.Add(preferenceData1);
            var preferenceDatatodelete = _context.UserPreferences.First();

            var result = await _userPreferenceRepository.Delete(preferenceDatatodelete.UserPreferenceID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(preferenceDatatodelete.UserPreferenceID, result.UserPreferenceID);

            var userInDb = await _context.Users.FindAsync(preferenceDatatodelete.UserPreferenceID);
            Assert.Null(userInDb);
        }

        [Test]
        public async Task Delete_UserPreferenceByUserID_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(_context);
            var preferenceData1 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference = 0
            };

            await _userPreferenceRepository.Add(preferenceData1);
            var preferenceDatatodelete = _context.UserPreferences.First();

            var result = await _userPreferenceRepository.DeleteByUserID("1");

            Assert.NotNull(result);

            result = await _userPreferenceRepository.DeleteByUserID("2");

            Assert.Null(result);
        }

        [Test]
        public async Task Delete_UserPreferenceByCategoryID_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(_context);
            var preferenceData1 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference = 0
            };

            await _userPreferenceRepository.Add(preferenceData1);
            var preferenceDatatodelete = _context.UserPreferences.First();

            var result = await _userPreferenceRepository.DeleteByCategoryID("1");

            Assert.NotNull(result);

            result = await _userPreferenceRepository.DeleteByCategoryID("2");

            Assert.Null(result);
        }


        [Test]
        public async Task Get_AllUserPreference_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(_context);
            var preferenceData1 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference=0
            };

            await _userPreferenceRepository.Add(preferenceData1);
            var result = await _userPreferenceRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }


        [Test]
        public async Task Get_AllByColumn_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(_context);
            var preferenceData1 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference= Preference.Like
            };

            var preferenceData2 = new UserPreference
            {
                CategoryID = 1,
                UserID = 2,
                preference = Preference.Like
            };

            var preferenceData3 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference = Preference.DisLike
            };


            await _userPreferenceRepository.Add(preferenceData1);
            await _userPreferenceRepository.Add(preferenceData2);
            await _userPreferenceRepository.Add(preferenceData3);

            var result1 = await _userPreferenceRepository.GetAll("UserPreferenceID", "2");


            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());
        }


        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _userPreferenceRepository = new UserPreferenceRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _userPreferenceRepository.GetAll("Title", "Economy"));
        }

        [Test]
        public async Task GetUserPreferenceById_ShouldReturnUserPreference()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UserPreferenceRepository(context);
            var preferenceData1 = new UserPreference
            {
                CategoryID = 1,
                UserID = 1,
                preference=0
            };
            context.UserPreferences.Add(preferenceData1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("UserPreferenceID", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.UserPreferenceID, Is.EqualTo(preferenceData1.UserPreferenceID));
        }
    }
}
