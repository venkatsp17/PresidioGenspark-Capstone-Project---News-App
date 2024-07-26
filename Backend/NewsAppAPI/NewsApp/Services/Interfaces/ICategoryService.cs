using NewsApp.DTOs;

namespace NewsApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategories();
    }
}
