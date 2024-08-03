using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using System.Linq.Expressions;
using static NewsApp.Models.Enum;

namespace NewsApp.Repositories.Classes
{
    public class UserRepository : IUserRepository
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(NewsAppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }

        public async Task<User> Add(User item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<User> Delete(string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();

            }
            return entity;
        }

        public async Task<User> Get(string key, string value)
        {
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id") && !(key.ToLower().Contains("oauth")))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(User), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<User, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);

            return result;
        }

        public async Task<IEnumerable<User>> GetAll(string key, string value)
        {
            if (key == "" && value == "")
            {
                var result1 = await _dbSet.ToListAsync();

                return result1;

            }
            object constant = value;
            if (key.ToLower().Contains("role"))
            {
                constant = System.Enum.Parse(typeof(UserType), value, true);
            }
            var parameter = Expression.Parameter(typeof(User), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, Expression.Constant(constant));
            var lambda = Expression.Lambda<Func<User, bool>>(equal, parameter);

            var result = await _dbSet.Where(lambda).ToListAsync();

            return result;

        }

        public Task<int> GetAllUserCountAsync()
        {
            return _dbSet.CountAsync(); 
        }

        public async Task<User> Update(User item, string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Update(item);
                await _context.SaveChangesAsync();
            }
            return item;
        }
    }
}
