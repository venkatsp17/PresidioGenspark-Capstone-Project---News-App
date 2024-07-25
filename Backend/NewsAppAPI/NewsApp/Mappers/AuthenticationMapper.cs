using NewsApp.DTOs;
using NewsApp.Models;

namespace NewsApp.Mappers
{
    public class AuthenticationMapper
    {
        public static LoginReturnDTO MapToLoginReturnDTO(User user)
        {
            return new LoginReturnDTO
            {
               UserID = user.UserID.ToString(),
               Email = user.Email,
               Name = user.Name,
               OAuthToken = user.OAuthToken,
               Role = user.Role,
            };
        }
    }
   
}
