using Google.Apis.Auth;
using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NewsApp.Services.Classes
{

    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly string _clientKey;
        [ExcludeFromCodeCoverage]
        public GoogleOAuthService(IConfiguration configuration)
        {
            _clientKey = configuration["CLIENT_KEY"];
        }
        [ExcludeFromCodeCoverage]
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
