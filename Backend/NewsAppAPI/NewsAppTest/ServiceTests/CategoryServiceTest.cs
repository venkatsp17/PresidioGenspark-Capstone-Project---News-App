using Moq;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAppTest.ServiceTests
{
    [TestFixture]
    public class CategoryServiceTest
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private ICategoryService _categoryService;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllCategories_ShouldReturnCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, Name = "Tech", Description = "Technology", Type = "General" },
                new Category { CategoryID = 2, Name = "Science", Description = "Science", Type = "General" }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAll("", "")).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllCategories();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Tech", result.First().Name);
            _categoryRepositoryMock.Verify(repo => repo.GetAll("", ""), Times.Once);
        }

        [Test]
        public void GetAllCategories_ShouldThrowNoAvailableItemException_WhenNoCategoriesExist()
        {
            // Arrange
            _categoryRepositoryMock.Setup(repo => repo.GetAll("", "")).ReturnsAsync((IEnumerable<Category>)null);

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _categoryService.GetAllCategories());
            _categoryRepositoryMock.Verify(repo => repo.GetAll("", ""), Times.Once);
        }

        [Test]
        public async Task AddCategory_ShouldReturnCategory_WhenCategoryIsAdded()
        {
            // Arrange
            var categoryGetDTO = new CategoryGetDTO { Name = "Health", Description = "Health" };
            var newCategory = new Category { CategoryID = 1, Name = "Health", Description = "Health", Type = "ADMIN_CATEGORY" };

            _categoryRepositoryMock.Setup(repo => repo.Get("Name", categoryGetDTO.Name)).ReturnsAsync((Category)null);
            _categoryRepositoryMock.Setup(repo => repo.Add(It.IsAny<Category>())).ReturnsAsync(newCategory);

            // Act
            var result = await _categoryService.AddCategory(categoryGetDTO);

            // Assert
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Health", result.Name);
            _categoryRepositoryMock.Verify(repo => repo.Get("Name", categoryGetDTO.Name), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.Add(It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public async Task AddCategory_ShouldReturnCategory_WhenCategoryNotAdded()
        {
            // Arrange
            var categoryGetDTO = new CategoryGetDTO { Name = "Health", Description = "Health" };
            var newCategory = new Category { CategoryID = 1, Name = "Health", Description = "Health", Type = "ADMIN_CATEGORY" };

            _categoryRepositoryMock.Setup(repo => repo.Get("Name", categoryGetDTO.Name)).ReturnsAsync((Category)null);
            _categoryRepositoryMock.Setup(repo => repo.Add(It.IsAny<Category>())).ReturnsAsync(new Category());

            Assert.ThrowsAsync<UnableToAddItemException>(async () => await _categoryService.AddCategory(categoryGetDTO));
        }

        [Test]
        public void AddCategory_ShouldThrowItemAlreadyExistException_WhenCategoryExists()
        {
            // Arrange
            var categoryGetDTO = new CategoryGetDTO { Name = "Health", Description = "Health" };
            var existingCategory = new Category { CategoryID = 1, Name = "Health", Description = "Health" };

            _categoryRepositoryMock.Setup(repo => repo.Get("Name", categoryGetDTO.Name)).ReturnsAsync(existingCategory);

            // Act & Assert
            Assert.ThrowsAsync<ItemAlreadyExistException>(async () => await _categoryService.AddCategory(categoryGetDTO));
            _categoryRepositoryMock.Verify(repo => repo.Get("Name", categoryGetDTO.Name), Times.Once);
        }

        [Test]
        public async Task DeleteCategory_ShouldReturnDeletedCategory_WhenCategoryIsDeleted()
        {
            // Arrange
            var category = new Category { CategoryID = 1, Name = "Health", Description = "Health" };

            _categoryRepositoryMock.Setup(repo => repo.Delete("1")).ReturnsAsync(category);

            // Act
            var result = await _categoryService.DeleteCategory(1);

            // Assert
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Health", result.Name);
            _categoryRepositoryMock.Verify(repo => repo.Delete("1"), Times.Once);
        }

        [Test]
        public void DeleteCategory_ShouldThrowItemNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _categoryRepositoryMock.Setup(repo => repo.Delete("1")).ReturnsAsync((Category)null);

            // Act & Assert
            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _categoryService.DeleteCategory(1));
            _categoryRepositoryMock.Verify(repo => repo.Delete("1"), Times.Once);
        }

        [Test]
        public async Task GetAllAdminCategories_ShouldReturnCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, Name = "Tech", Description = "Technology", Type = "General" },
                new Category { CategoryID = 2, Name = "Science", Description = "Science", Type = "General" }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAll("", "")).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllAdminCategories(0);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Tech", result.First().Name);
            _categoryRepositoryMock.Verify(repo => repo.GetAll("", ""), Times.Once);
        }

        [Test]
        public async Task GetAllAdminCategories_ShouldReturnCategoriesByArticleId_WhenArticleIdIsProvided()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, Name = "Tech", Description = "Technology", Type = "General" }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetCategoriesByArticleIdAsync(1)).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllAdminCategories(1);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Tech", result.First().Name);
            _categoryRepositoryMock.Verify(repo => repo.GetCategoriesByArticleIdAsync(1), Times.Once);
        }

        [Test]
        public void GetAllAdminCategories_ShouldThrowNoAvailableItemException_WhenNoCategoriesExist()
        {
            // Arrange
            _categoryRepositoryMock.Setup(repo => repo.GetAll("", "")).ReturnsAsync((IEnumerable<Category>)null);

            // Act & Assert
            Assert.ThrowsAsync<NoAvailableItemException>(async () => await _categoryService.GetAllAdminCategories(0));
            _categoryRepositoryMock.Verify(repo => repo.GetAll("", ""), Times.Once);
        }
    }
}
