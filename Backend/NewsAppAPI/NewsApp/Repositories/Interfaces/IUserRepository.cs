using NewsApp.Models;

namespace NewsApp.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<string,User,string>
    {
        Task<int> GetAllUserCountAsync();
    }
}
