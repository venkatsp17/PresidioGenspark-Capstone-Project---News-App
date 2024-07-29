using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories;
using NewsApp.Repositories.Classes;


namespace NewsAppTest.RepositoryTests
{
    public class CommentRepositoryTest
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
        public async Task Add_Comment_Success()
        {
            var context = GetInMemoryDbContext();
            var _commentRepository = new CommentRepository(context);
            var comment1 = new Comment
            {
                CommentID = 1,
                ArticleID = 1,
                UserID = 1,
                Content = "Great article! Really enjoyed the insights.",
                Timestamp = DateTime.UtcNow
            };

            var result = await _commentRepository.Add(comment1);

            Assert.NotNull(result);
            Assert.AreEqual(comment1.CommentID, result.CommentID);
        }

        [Test]
        public async Task Delete_Comment_Success()
        {
            var _context = GetInMemoryDbContext();
            var _commentRepository = new CommentRepository(_context);
            var comment1 = new Comment
            {
                CommentID = 1,
                ArticleID = 1,
                UserID = 1,
                Content = "Great article! Really enjoyed the insights.",
                Timestamp = DateTime.UtcNow
            };

            await _commentRepository.Add(comment1);
            var articltodelete = _context.Comments.First();

            var result = await _commentRepository.Delete(articltodelete.CommentID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(articltodelete.CommentID, result.CommentID);

            var userInDb = await _context.Users.FindAsync(articltodelete.CommentID);
            Assert.Null(userInDb);
        }

      

        [Test]
        public async Task Update_Comment_Success()
        {
            var _context = GetInMemoryDbContext();
            var _commentRepository = new CommentRepository(_context);
            var comment1 = new Comment
            {
                CommentID = 1,
                ArticleID = 1,
                UserID = 1,
                Content = "Great article! Really enjoyed the insights.",
                Timestamp = DateTime.UtcNow
            };

            await _commentRepository.Add(comment1);
            var commentToUpdate = _context.Comments.First();
            commentToUpdate.Content = "Worst article! Really enjoyed the insights.";

            var result = await _commentRepository.Update(commentToUpdate, commentToUpdate.CommentID.ToString());

            Assert.NotNull(result);
            Assert.AreEqual("Worst article! Really enjoyed the insights.", result.Content);

            var commentInDb = await _context.Comments.FindAsync(commentToUpdate.CommentID);
            Assert.NotNull(commentInDb);
            Assert.AreEqual("Worst article! Really enjoyed the insights.", commentInDb.Content);
        }

    
        [Test]
        public async Task Get_AllComment_Success()
        {
            var _context = GetInMemoryDbContext();
            var _commentRepository = new CommentRepository(_context);
            var comment1 = new Comment
            {
                CommentID = 1,
                ArticleID = 1,
                UserID = 1,
                Content = "Great article! Really enjoyed the insights.",
                Timestamp = DateTime.UtcNow
            };

            await _commentRepository.Add(comment1);
            var result = await _commentRepository.GetAll("", "");

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
        }

  
        [Test]
        public async Task Get_AllByColumn_Success()
        {
            var _context = GetInMemoryDbContext();
            var _commentRepository = new CommentRepository(_context);
            var comment1 = new Comment
            {
                CommentID = 1,
                ArticleID = 1,
                UserID = 1,
                Content = "Great article! Really enjoyed the insights.",
                Timestamp = DateTime.UtcNow
            };

            var comment2 = new Comment
            {
                CommentID = 2,
                ArticleID = 1,
                UserID = 2,
                Content = "I disagree with some points, but overall a good read.",
                Timestamp = DateTime.UtcNow
            };

            var comment3 = new Comment
            {
                CommentID = 3,
                UserID = 3,
                Content = "Very informative. Looking forward to more articles like this.",
                Timestamp = DateTime.UtcNow
            };


            await _commentRepository.Add(comment1);
            await _commentRepository.Add(comment2);
            await _commentRepository.Add(comment3);
            var result = await _commentRepository.GetAll("Content", "article");

            var result1 = await _commentRepository.GetAll("CommentID", "3");

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.NotNull(result1);
            Assert.AreEqual(1, result1.Count());
        }


    

        [Test]
        public async Task Get_AllByColumn_ColumnNotExistException()
        {
            var _context = GetInMemoryDbContext();
            var _commentRepository = new CommentRepository(_context);

            Assert.ThrowsAsync<ColumnNotExistException>(async () => await _commentRepository.GetAll("Title", "Economy"));
        }

        [Test]
        public async Task GetCommentById_ShouldReturnComment()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CommentRepository(context);
            var comment1 = new Comment
            {
                CommentID = 1,
                ArticleID = 1,
                UserID = 1,
                Content = "Great article! Really enjoyed the insights.",
                Timestamp = DateTime.UtcNow
            };
            context.Comments.Add(comment1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Get("CommentID", "1");

            // Assert
            Assert.NotNull(result);
            Assert.That(result.CommentID, Is.EqualTo(comment1.CommentID));
        }

    
    }
}
