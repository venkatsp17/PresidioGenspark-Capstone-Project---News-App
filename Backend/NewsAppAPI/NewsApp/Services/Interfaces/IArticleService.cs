using NewsApp.DTOs;
using NewsApp.Models;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<AdminArticleReturnDTO>> GetTopStoryArticlesAsync(int pageNumber = 1, int pageSize = 10);

        Task<AdminArticleReturnDTO> ChangeArticleStatus(string articleId, ArticleStatus articleStatus);
    }
}
