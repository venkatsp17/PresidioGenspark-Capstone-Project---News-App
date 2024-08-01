using NewsApp.DTOs;

namespace NewsApp.Services.Interfaces
{
    public interface IUserPreferenceService
    {
        Task<IEnumerable<UserPreferenceReturnDTO>> AddPreferences(UserPreferenceDTO userPreferenceDTO);

        Task<IEnumerable<UserPreferenceReturnDTO>> GetUserPreferences(int userid);
    }
}
