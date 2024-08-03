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
using static NewsApp.Models.Enum;

namespace NewsAppTest.ServiceTests
{
    [TestFixture]
    public class UserPreferenceServiceTest
    {
        private Mock<IUserPreferenceRepository> _userPreferenceRepositoryMock;
        private UserPreferenceService _userPreferenceService;

        [SetUp]
        public void SetUp()
        {
            _userPreferenceRepositoryMock = new Mock<IUserPreferenceRepository>();
            _userPreferenceService = new UserPreferenceService(_userPreferenceRepositoryMock.Object);
        }

        [Test]
        public async Task AddPreferences_ShouldAddPreferencesCorrectly()
        {
            // Arrange
            var userPreferenceDTO = new UserPreferenceDTO
            {
                UserID = 1,
                preferences = new Dictionary<int, Preference>
                {
                    { 1, Preference.Like },
                }
            };

            _userPreferenceRepositoryMock
                .Setup(repo => repo.DeleteByUserID(It.IsAny<string>()))
                .ReturnsAsync(new List<UserPreference> { });


            _userPreferenceRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<UserPreference>()))
                .ReturnsAsync(
                   new UserPreference(){preference=Preference.Like,CategoryID=1, UserID=1, UserPreferenceID=1 }
                    );

          

            // Act
            var result = await _userPreferenceService.AddPreferences(userPreferenceDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.First().ID);
            Assert.AreEqual(1, result.First().CategoryID);
            Assert.AreEqual(Preference.Like, result.First().preference);
            Assert.AreEqual(1, result.First().UserID);
        }

        [Test]
        public async Task AddPreferences_ShouldThrowException_WhenUnableToAddPreference()
        {
            // Arrange
            var userPreferenceDTO = new UserPreferenceDTO
            {
                UserID = 1,
                preferences = new Dictionary<int, Preference>
                {
                    { 1, Preference.Like },
                    { 2, Preference.DisLike }
                }
            };

            _userPreferenceRepositoryMock
                   .Setup(repo => repo.DeleteByUserID(It.IsAny<string>()))
                   .ReturnsAsync(new List<UserPreference> { });

            _userPreferenceRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<UserPreference>()))
                .ReturnsAsync((UserPreference)null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToAddItemException>(() => _userPreferenceService.AddPreferences(userPreferenceDTO));
        }

        [Test]
        public async Task GetUserPreferences_ShouldReturnUserPreferences()
        {
            // Arrange
            int userId = 1;
            var userPreferences = new List<UserPreference>
            {
                new UserPreference { UserPreferenceID = 1, UserID = userId, CategoryID = 1, preference = Preference.Like },
                new UserPreference { UserPreferenceID = 2, UserID = userId, CategoryID = 2, preference = Preference.DisLike }
            };

            _userPreferenceRepositoryMock
                .Setup(repo => repo.GetAll("UserID", userId.ToString()))
                .ReturnsAsync(userPreferences);

            // Act
            var result = await _userPreferenceService.GetUserPreferences(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            var resultList = result.ToList();

            Assert.AreEqual(1, resultList[0].ID);
            Assert.AreEqual(1, resultList[0].CategoryID);
            Assert.AreEqual(Preference.Like, resultList[0].preference);
            Assert.AreEqual(userId, resultList[0].UserID);

            Assert.AreEqual(2, resultList[1].ID);
            Assert.AreEqual(2, resultList[1].CategoryID);
            Assert.AreEqual(Preference.DisLike, resultList[1].preference);
            Assert.AreEqual(userId, resultList[1].UserID);
        }
    }
}
