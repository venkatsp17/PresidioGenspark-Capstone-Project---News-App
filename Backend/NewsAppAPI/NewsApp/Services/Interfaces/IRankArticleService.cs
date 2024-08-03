using NewsApp.DTOs;

namespace NewsApp.Services.Interfaces
{
    public interface IRankArticleService
    {
        Task<IEnumerable<AdminArticleReturnDTO>> RankTop3Articles(int category, int userid);

        Task<StatisticsDto> GetAllStatistics();
    }
}
