using NewsApp.Models;

namespace NewsApp.Repositories.Interfaces
{
    public interface IArticleCategoryRepository : IRepository<string, ArticleCategory, string>
    {
        public Task<IEnumerable<ArticleCategory>> DeleteByArticleID(string key);

        public Task<IEnumerable<ArticleCategory>> DeleteByCategoryID(string key);
    }
}
