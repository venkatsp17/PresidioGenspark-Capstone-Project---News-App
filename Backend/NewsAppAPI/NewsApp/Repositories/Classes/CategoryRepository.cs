using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NewsApp.Repositories.Classes
{
    public class CategoryRepository : IRepository<string, Category, string>
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<Category> _dbSet;

        public CategoryRepository(NewsAppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<Category>();
        }

        public async Task<Category> Add(Category item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Category> Delete(string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<Category> Get(string key, string value)
        {
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id"))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(Category), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<Category, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);
            return result;
        }

        public async Task<IEnumerable<Category>> GetAll(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                var result1 = await _dbSet.ToListAsync();
                return result1;
            }

            var propertyInfo = typeof(Category).GetProperty(key);
            if (propertyInfo == null)
            {
                throw new ColumnNotExistException(key, "Category");
            }

            var parameter = Expression.Parameter(typeof(Category), "e");
            var property = Expression.Property(parameter, key);

            Expression<Func<Category, bool>> lambda;

            if (key.ToLower().Contains("id"))
            {
                var constantValue = Expression.Constant(int.Parse(value));
                var equalExpression = Expression.Equal(property, constantValue);
                lambda = Expression.Lambda<Func<Category, bool>>(equalExpression, parameter);
            }
            else
            {
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var propertyToLower = Expression.Call(property, toLowerMethod);
                var constantValue = Expression.Constant(value.ToLower());
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(propertyToLower, containsMethod, constantValue);
                lambda = Expression.Lambda<Func<Category, bool>>(containsExpression, parameter);
            }

            var result = await _dbSet.Where(lambda).ToListAsync();

            return result;
        }

        public async Task<Category> Update(Category item, string key)
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

