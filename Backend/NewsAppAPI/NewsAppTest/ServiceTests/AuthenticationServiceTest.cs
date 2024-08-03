using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NewsApp.Contexts;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Classes;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using static NewsApp.Models.Enum;

namespace NewsAppTest.NewFolder
{
    public class AuthenticationServiceTest
    {
       
        [Test]
        public async Task UserLogin_Successful_ReturnsLoginReturnDTO()
        {
            // Arrange
            var loginDTO = new LoginGetDTO1 { Email = "test@example.com", Password = "password123" };
            var user = new User
            {
                UserID = 1,
                Email = "test@example.com",
                Name = "Test User",
                Role = UserType.Reader,
                Password_Hashkey = new byte[64], // HMACSHA512 key length
                Password = new HMACSHA512(new byte[64]).ComputeHash(System.Text.Encoding.UTF8.GetBytes("password123"))
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.Get("Email", loginDTO.Email)).ReturnsAsync(user);

            var mockTokenService = new Mock<ITokenService>();
            mockTokenService.Setup(service => service.GenerateToken(It.IsAny<User>())).Returns("dummyToken");

            var authenticationService = new AuthenticationService(mockUserRepository.Object, null, mockTokenService.Object);

            // Act
            var result = await authenticationService.UserLogin(loginDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.AreEqual("dummyToken", result.Token);
        }

        [Test]
        public void UserLogin_UserNotFound_ThrowsUnableToLoginException()
        {
            // Arrange
            var loginDTO = new LoginGetDTO1 { Email = "test@example.com", Password = "password123" };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.Get("Email", loginDTO.Email)).ReturnsAsync((User)null);

            var authenticationService = new AuthenticationService(mockUserRepository.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToLoginException>(async () => await authenticationService.UserLogin(loginDTO));
        }

        [Test]
        public void UserLogin_IncorrectPassword_ThrowsUnauthorizedUserException()
        {
            // Arrange
            var loginDTO = new LoginGetDTO1 { Email = "test@example.com", Password = "wrongPassword" };
            var user = new User
            {
                UserID = 1,
                Email = "test@example.com",
                Name = "Test User",
                Role = UserType.Reader,
                Password_Hashkey = new byte[64], // HMACSHA512 key length
                Password = new HMACSHA512(new byte[64]).ComputeHash(Encoding.UTF8.GetBytes("password123"))
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.Get("Email", loginDTO.Email)).ReturnsAsync(user);

            var authenticationService = new AuthenticationService(mockUserRepository.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedUserException>(async () => await authenticationService.UserLogin(loginDTO));
        }

        [Test]
        public async Task UserRegister_Successful_ReturnsRegisterReturnDTO()
        {
            // Arrange
            var registerDTO = new RegisterGetDTO { Email = "new@example.com", Password = "password123", Name = "New User" };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.Get("Email", registerDTO.Email)).ReturnsAsync((User)null);
            mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(new User
            {
                UserID = 1,
                Email = "new@example.com",
                Name = "New User",
                Role = UserType.Reader
            });

            var authenticationService = new AuthenticationService(mockUserRepository.Object, null, null);

            // Act
            var result = await authenticationService.UserRegister(registerDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("new@example.com", result.Email);
            Assert.AreEqual("New User", result.Name);
        }

        [Test]
        public void UserRegister_UserAlreadyExists_ThrowsUserAlreadyExistsException()
        {
            // Arrange
            var registerDTO = new RegisterGetDTO { Email = "existing@example.com", Password = "password123", Name = "Existing User" };
            var existingUser = new User { Email = "existing@example.com" };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.Get("Email", registerDTO.Email)).ReturnsAsync(existingUser);

            var authenticationService = new AuthenticationService(mockUserRepository.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await authenticationService.UserRegister(registerDTO));
        }

        [Test]
        public void UserRegister_UnableToRegister_ThrowsUnableToRegisterException()
        {
            // Arrange
            var registerDTO = new RegisterGetDTO { Email = "new@example.com", Password = "password123", Name = "New User" };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.Get("Email", registerDTO.Email)).ReturnsAsync((User)null);
            mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(new User());

            var authenticationService = new AuthenticationService(mockUserRepository.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<UnableToRegisterException>(async () => await authenticationService.UserRegister(registerDTO));
        }

    }
}
