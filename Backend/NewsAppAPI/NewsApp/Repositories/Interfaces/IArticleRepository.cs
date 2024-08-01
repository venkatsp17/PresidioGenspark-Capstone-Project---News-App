using NewsApp.Models;
using System.Threading.Tasks;
using static NewsApp.Models.Enum;

namespace NewsApp.Repositories.Interfaces
{
    public interface IArticleRepository : IRepository<string, Article, string>
    {

        Task<IEnumerable<Article>> GetAllByStatusAndCategoryAsync(ArticleStatus article, int categoryID);

        Task<IEnumerable<Article>> GetAllApprcvedEditedArticlesAndCategoryAsync(int categoryID);

        Task<IEnumerable<Article>> GetArticlesForRanking(int categoryID);

        Task<IEnumerable<Article>> GetAllApprcvedEditedArticlesAsync();

    }
}
