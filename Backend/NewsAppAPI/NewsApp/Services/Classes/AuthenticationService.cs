using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Mappers;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<string, User, string> _userRepository;
        private readonly IGoogleOAuthService _googleOAuthService;

        public AuthenticationService(IRepository<string, User, string> userRepository, IGoogleOAuthService googleOAuthService)
        {
            _userRepository = userRepository;
            _googleOAuthService = googleOAuthService;
        }


        public async Task<LoginReturnDTO> LoginUser(LoginGetDTO loginGetDTO)
        {
            var payload = await _googleOAuthService.ValidateGoogleTokenAsync(loginGetDTO.oAuthToken);

            try
            {
                var user = await _userRepository.Get("OAuthID", payload.Subject);
                if (user.OAuthToken != loginGetDTO.oAuthToken)
                {
                    user.OAuthToken = loginGetDTO.oAuthToken;
                    await _userRepository.Update(user, user.UserID.ToString());
                }
                return AuthenticationMapper.MapToLoginReturnDTO(user);
            }
            catch(ItemNotFoundException)
            {

                var user = new User
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    OAuthID = payload.Subject,
                    OAuthToken = loginGetDTO.oAuthToken,
                    Role = UserType.Reader
                };

                var result = await _userRepository.Add(user);
                if (result == null)
                {
                    throw new UnableToAddUserException();
                }
                return AuthenticationMapper.MapToLoginReturnDTO(result);
            }
        }

        public async Task LogoutUser(string userId)
        {
            var user = await _userRepository.Get("UserID", userId);
            if (user == null)
            {
                throw new ItemNotFoundException();
            }

            user.OAuthToken = null;
            await _userRepository.Update(user, userId);
        }
    }
}
