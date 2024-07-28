using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NewsApp.Contexts;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Classes;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;

using static NewsApp.Models.Enum;

namespace NewsAppTest.NewFolder
{
    public class AuthenticationServiceTest
    {
        private readonly Mock<IGoogleOAuthService> _mockGoogleOAuthService;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTest()
        {
            _mockGoogleOAuthService = new Mock<IGoogleOAuthService>();
        }

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
        public async Task LoginUser_UserExists_UpdatesToken_ReturnsUser()
        {
            // Arrange
            var loginGetDTO = new LoginGetDTO { oAuthToken = "newToken" };
            var payload = new GoogleJsonWebSignature.Payload
            {
                Subject = "testSubject",
                Email = "test@example.com",
                Name = "Test User"
            };
            _mockGoogleOAuthService.Setup(x => x.ValidateGoogleTokenAsync(It.IsAny<string>()))
                                   .ReturnsAsync(payload);

            var context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(context);
            _userRepository.Add(new User { UserID = 1, OAuthID = "testSubject", OAuthToken = "oldToken", Email = "test@example.com", Name = "Test User", Role = UserType.Reader });
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);
            var authenticationService = new AuthenticationService(_userRepository, _mockGoogleOAuthService.Object, service);
            var result = await authenticationService.LoginUser(loginGetDTO);


            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
            //Assert.AreEqual("newToken", result.Token);
        }

        [Test]
        public async Task LoginUser_UserDoesNotExist_AddsUser_ReturnsUser()
        {
            // Arrange
            var loginGetDTO = new LoginGetDTO { oAuthToken = "newToken" };
            var payload = new GoogleJsonWebSignature.Payload
            {
                Subject = "newSubject",
                Email = "new@example.com",
                Name = "New User"
            };
            _mockGoogleOAuthService.Setup(x => x.ValidateGoogleTokenAsync(It.IsAny<string>()))
                                   .ReturnsAsync(payload);

            // Act
            LoginReturnDTO result;

            var context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(context);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);
            var authenticationService = new AuthenticationService(_userRepository, _mockGoogleOAuthService.Object, service);
            result = await authenticationService.LoginUser(loginGetDTO);


            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("new@example.com", result.Email);
            //Assert.AreEqual("newToken", result.OAuthToken);


            var user = await _userRepository.Get("OAuthID", "newSubject");
            Assert.NotNull(user);
            Assert.AreEqual("new@example.com", user.Email);

        }

        [Test]
        public async Task LogoutUser_UserExists_UpdatesTokenToNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(context);
            _userRepository.Add(new User { UserID = 1, OAuthID = "testSubject", OAuthToken = "oldToken", Email = "test@example.com", Name = "Test User", Role = UserType.Reader });

            // Act
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);
            var authenticationService = new AuthenticationService(_userRepository, _mockGoogleOAuthService.Object, service);
            await authenticationService.LogoutUser("1");

            // Assert
            var user = await _userRepository.Get("UserID","1");
            Assert.NotNull(user);
            Assert.Null(user.OAuthToken);

        }

        [Test]
        public async Task LogoutUser_UserDoesNotExist_ThrowsItemNotFoundException()
        {
            // Act & Assert
            var context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(context);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);
            var authenticationService = new AuthenticationService(_userRepository, _mockGoogleOAuthService.Object, service);
            Assert.ThrowsAsync<ItemNotFoundException>(async () => await authenticationService.LogoutUser("999"));

        }
    }
}
