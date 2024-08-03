using Microsoft.IdentityModel.Tokens;
using NewsApp.Models;
using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewsApp.Services.Classes
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _key;

        [ExcludeFromCodeCoverage]
        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration["JWT"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        }
        [ExcludeFromCodeCoverage]
        public string GenerateToken(User user)
        {
            string token = string.Empty;
            var claims = new List<Claim>()
            {
                new Claim("UID", user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var myToken = new JwtSecurityToken(null, null, claims, expires: DateTime.Now.AddHours(12), signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(myToken);
            return token;
        }
    }
}
