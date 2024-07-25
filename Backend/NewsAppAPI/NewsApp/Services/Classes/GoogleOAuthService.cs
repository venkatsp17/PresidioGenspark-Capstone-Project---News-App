using Google.Apis.Auth;
using NewsApp.Services.Interfaces;

namespace NewsApp.Services.Classes
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly string _clientKey;

        public GoogleOAuthService(IConfiguration configuration)
        {
            _clientKey = configuration["CLIENT_KEY"];
        }
        public async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _clientKey }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
    }
}
