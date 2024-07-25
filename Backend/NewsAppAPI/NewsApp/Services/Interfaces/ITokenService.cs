using NewsApp.Models;

namespace NewsApp.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}
