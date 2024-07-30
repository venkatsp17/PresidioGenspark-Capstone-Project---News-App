using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using System.Linq.Expressions;

namespace NewsApp.Repositories.Classes
{
    public class SavedArticleRepository : ISavedRepository
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

        public async Task<SavedArticle> GetBy2Id(string key1, string value1, string key2, string value2)
        {

                var constant1 = Expression.Constant(value1);
                var constant2 = Expression.Constant(value2);

                if (key1.ToLower().Contains("id"))
                {
                    constant1 = Expression.Constant(int.Parse(value1));
                }

                if (key2.ToLower().Contains("id"))
                {
                    constant2 = Expression.Constant(int.Parse(value2));
                }

                var parameter = Expression.Parameter(typeof(SavedArticle), "e");
                var property1 = Expression.Property(parameter, key1);
                var property2 = Expression.Property(parameter, key2);
                var equal1 = Expression.Equal(property1, constant1);
                var equal2 = Expression.Equal(property2, constant2);
                var and = Expression.AndAlso(equal1, equal2);
                var lambda = Expression.Lambda<Func<SavedArticle, bool>>(and, parameter);

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
