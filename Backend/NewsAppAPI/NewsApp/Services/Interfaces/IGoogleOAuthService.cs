using Google.Apis.Auth;

namespace NewsApp.Services.Interfaces
{
    public interface IGoogleOAuthService
    {
        Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token);
    }
}
