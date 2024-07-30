using NewsApp.DTOs;
using NewsApp.Models;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Interfaces
{
    public interface IArticleService
    {
        Task FetchAndSaveCategoryArticlesAsync();
        Task FetchAndSaveArticlesAsync();

        Task<AdminArticleReturnDTO> ChangeArticleStatus(string articleId, ArticleStatus articleStatus);

        Task<AdminArticleReturnDTO> EditArticleData(AdminArticleReturnDTO adminArticleReturnDTO);

        Task<AdminArticlePaginatedReturnDTO> GetPaginatedArticlesAsync(int pageNumber, int pageSize, string status, int categoryID);

        Task<AdminArticlePaginatedReturnDTO> GetPaginatedArticlesForUserAsync(int pageNumber, int pageSize, int categoryID, int userid);

        Task<ShareDataReturnDTO> UpdateShareCount(ShareDataDTO shareDataDTO);

    }
}
