using NewsApp.DTOs;
using NewsApp.Models;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Interfaces
{
    public interface IArticleService
    {
        Task FetchAndSaveArticlesAsync();

        Task<AdminArticleReturnDTO> ChangeArticleStatus(string articleId, ArticleStatus articleStatus);

        Task<AdminArticlePaginatedReturnDTO> GetPaginatedArticlesAsync(int pageNumber, int pageSize, string status);
    }
}
