using NewsApp.DTOs;

namespace NewsApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategories();

        Task<CategoryDTO> AddCategory(CategoryGetDTO categoryGetDTO);

        Task<CategoryDTO> DeleteCategory(int categoryid);
    }
}
