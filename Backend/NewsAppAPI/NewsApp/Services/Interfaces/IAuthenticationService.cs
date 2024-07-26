using NewsApp.DTOs;


namespace NewsApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginReturnDTO> LoginUser(LoginGetDTO loginGetDTO);
        Task<RegisterReturnDTO> CreatePassword(CreatePasswordDTO createPasswordDTO);
        Task LogoutUser(string userId);
        Task<RegisterReturnDTO> UserRegister(RegisterGetDTO userRegisterDTO);
        Task<LoginReturnDTO> UserLogin(LoginGetDTO1 userLoginDTO);
    }
}
