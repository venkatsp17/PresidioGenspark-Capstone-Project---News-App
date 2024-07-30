using NewsApp.DTOs;

namespace NewsApp.Services.Interfaces
{
    public interface ISavedArticleService
    {
        Task<int> SaveAndUnSaveArticle(int articleid, int userid);

        Task<bool> CheckForSaved(int articleid, int userid);

        Task<AdminArticlePaginatedReturnDTO> GetAllSavedArticles(int userid, int pageNumber, int pageSize, string query);
    }
}
