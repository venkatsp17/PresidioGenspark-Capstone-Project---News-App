using NewsApp.Models;

namespace NewsApp.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<string, Category, string>
    {
        Task<IEnumerable<Category>> GetCategoriesByArticleIdAsync(int articleId);
    }
}

