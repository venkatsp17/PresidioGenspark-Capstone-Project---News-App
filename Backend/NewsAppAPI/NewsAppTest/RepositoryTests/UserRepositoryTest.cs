using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories;
using NewsApp.Repositories.Classes;
using System;
using System.Text;
using static NewsApp.Models.Enum;

namespace NewsAppTest.RepositoryTests
{
    public class UserRepositoryTest
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
        public async Task Add_User_Success()
        {
            var context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(context);
            var newUser = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 3, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };

            var result = await _userRepository.Add(newUser);

            Assert.NotNull(result);
            Assert.AreEqual(newUser.UserID, result.UserID);
        }

        [Test]
        public async Task Delete_User_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(_context);
            var newUser = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 3, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };

            await _userRepository.Add(newUser);
            var userToDelete = _context.Users.First();

            var result = await _userRepository.Delete(userToDelete.UserID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(userToDelete.UserID, result.UserID);

            var userInDb = await _context.Users.FindAsync(userToDelete.UserID);
            Assert.Null(userInDb);
        }

    

        [Test]
        public async Task Update_User_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(_context);
            var newUser = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 3, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };

            await _userRepository.Add(newUser);
            var userToUpdate = _context.Users.First();
            userToUpdate.Email = "news@gmail.com";

            var result = await _userRepository.Update(userToUpdate, userToUpdate.UserID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual("news@gmail.com", result.Email);

            var userInDb = await _context.Users.FindAsync(userToUpdate.UserID);
            Assert.NotNull(userInDb);
            Assert.AreEqual("news@gmail.com", userInDb.Email);
        }

    

        [Test]
        public async Task Get_AllUsers_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(_context);
            var newUser = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 3, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };

            await _userRepository.Add(newUser);
            var result = await _userRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }

      

        [Test]
        public async Task Get_AllByRole_Success()
        {
            var _context = GetInMemoryDbContext();
            var _userRepository = new UserRepository(_context);
            var newUser = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 3, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };
            var newUser1 = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 4, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };
            var newUser2 = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Admin, UserID = 5, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };

            await _userRepository.Add(newUser);
            await _userRepository.Add(newUser1);
            await _userRepository.Add(newUser2);
            var result = await _userRepository.GetAll("Role", "0");

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }


    

        [Test]
        public async Task GetUserById_ShouldReturnUser()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UserRepository(context);
            var user = new User { Email = "sales@gmail.com", Name = "news", OAuthID = "35fdsf6dts76fd6fsd", OAuthToken = "35fdsf6dts76fd6fsd", Role = UserType.Reader, UserID = 3, Password = Encoding.UTF8.GetBytes("password"), Password_Hashkey = Encoding.UTF8.GetBytes("passwordHash") };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("UserID", "3");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }

    
    }
}