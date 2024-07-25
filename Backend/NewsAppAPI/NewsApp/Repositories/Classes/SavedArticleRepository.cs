using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NewsApp.Repositories.Classes
{
    public class SavedArticleRepository : IRepository<string, SavedArticle, string>
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<SavedArticle> _dbSet;

        public SavedArticleRepository(NewsAppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<SavedArticle>();
        }

        public async Task<SavedArticle> Add(SavedArticle item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<SavedArticle> Delete(string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<SavedArticle> Get(string key, string value)
        {
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id"))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(SavedArticle), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<SavedArticle, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);

            return result;

        }

        public async Task<IEnumerable<SavedArticle>> GetAll(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                var result1 = await _dbSet.ToListAsync();

                return result1;

            }

            var propertyInfo = typeof(SavedArticle).GetProperty(key);
            if (propertyInfo == null)
            {
                throw new ColumnNotExistException(key, "SavedArticle");
            }

            var parameter = Expression.Parameter(typeof(SavedArticle), "e");
            var property = Expression.Property(parameter, key);

            Expression<Func<SavedArticle, bool>> lambda;

            var constantValue = Expression.Constant(int.Parse(value));
            var equalExpression = Expression.Equal(property, constantValue);
            lambda = Expression.Lambda<Func<SavedArticle, bool>>(equalExpression, parameter);

            var result = await _dbSet.Where(lambda).ToListAsync();


            return result;

        }

        public async Task<SavedArticle> Update(SavedArticle item, string key)
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
