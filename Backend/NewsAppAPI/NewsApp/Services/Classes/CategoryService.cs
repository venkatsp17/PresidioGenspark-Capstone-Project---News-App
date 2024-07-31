using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;

namespace NewsApp.Services.Classes
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<string, Category, string> _categoryRepository;
        public CategoryService(IRepository<string, Category, string> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAll("", "");

            if(categories == null || categories.Count() == 0)
            {
                throw new NoAvailableItemException();
            }

            return categories.Select(c => new CategoryDTO { Id=c.CategoryID, Name=c.Name,Description=c.Description });
        }

        public async Task<CategoryDTO> AddCategory(CategoryGetDTO categoryGetDTO)
        {
            var newCategory = new Category()
            {
                Description = categoryGetDTO.Description,
                Name = categoryGetDTO.Name,
                Type = "ADMIN_CATEGORY"
            };
            var category = await _categoryRepository.Add(newCategory);

            if (category.CategoryID == null)
            {
                throw new UnableToAddItemException();
            }

            return new CategoryDTO()
            {
                Id = category.CategoryID,
                Description= category.Description,
                Name = category.Name,
            };
        }

        public async Task<CategoryDTO> DeleteCategory(int categoryid)
        {
            var category = await _categoryRepository.Delete(categoryid.ToString());

            if (category.CategoryID == null)
            {
                throw new ItemNotFoundException();
            }

            return new CategoryDTO()
            {
                Id = category.CategoryID,
                Description = category.Description,
                Name = category.Name,
            };
        }
    }
}
