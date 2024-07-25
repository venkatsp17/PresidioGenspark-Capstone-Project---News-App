using NewsApp.DTOs;
using NewsApp.Models;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginReturnDTO> LoginUser(LoginGetDTO loginGetDTO);
        Task LogoutUser(string userId);
    }
}
