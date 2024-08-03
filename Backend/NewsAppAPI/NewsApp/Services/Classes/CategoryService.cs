using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;

namespace NewsApp.Services.Classes
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAll("", "");

            if(!categories.Any())
            {
                throw new NoAvailableItemException();
            }

            return categories.Select(c => new CategoryDTO { Id=c.CategoryID, Name=c.Name,Description=c.Description, Type=c.Type });
        }

        public async Task<CategoryDTO> AddCategory(CategoryGetDTO categoryGetDTO)
        {
            var existingCategory = await _categoryRepository.Get("Name",categoryGetDTO.Name);
            if (existingCategory != null)
            {
                throw new ItemAlreadyExistException();
            }

            var newCategory = new Category()
            {
                Description = categoryGetDTO.Description,
                Name = categoryGetDTO.Name,
                Type = "ADMIN_CATEGORY"
            };
            var category = await _categoryRepository.Add(newCategory);

            if (category.CategoryID == 0)
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

            if (category == null)
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

        public async Task<IEnumerable<CategoryAdminDTO>> GetAllAdminCategories(int articleid)
        {
            IEnumerable<Category> categories;
            if (articleid == 0)
            {
                categories = await _categoryRepository.GetAll("", "");
               
            }
            else
            {
                categories = await _categoryRepository.GetCategoriesByArticleIdAsync(articleid);
            }

            if (categories == null || categories.Count() == 0)
            {
                throw new NoAvailableItemException();
            }

            return categories.Select(c => new CategoryAdminDTO { Id = c.CategoryID, Name = c.Name, Description = c.Description, Type = c.Type });

        }

    }
}
