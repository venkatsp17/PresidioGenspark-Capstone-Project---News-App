using NewsApp.Models;

namespace NewsApp.Repositories.Interfaces
{
    public interface ISavedRepository : IRepository<string,SavedArticle, string>
    {
        Task<SavedArticle> GetBy2Id(string key1, string value1, string key2, string value2);
    }
}
