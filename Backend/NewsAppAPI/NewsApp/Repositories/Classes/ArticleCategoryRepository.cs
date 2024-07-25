using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace NewsApp.Repositories.Classes
{
    public class ArticleCategoryRepository : IArticleCategoryRepository
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<ArticleCategory> _dbSet;

        public ArticleCategoryRepository(NewsAppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<ArticleCategory>();
        }

        public async Task<ArticleCategory> Add(ArticleCategory item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<ArticleCategory> Delete(string key)
        {
            // Check if the key contains a hyphen, indicating a composite key
            if (key.Contains('-'))
            {
                var keys = key.Split('-');
                if (keys.Length != 2)
                {
                    throw new ArgumentException("The key must be provided in the format 'ArticleID-CategoryID'.");
                }

                int articleId = int.Parse(keys[0]);
                int categoryId = int.Parse(keys[1]);

                var entity = await _dbSet.FindAsync(articleId, categoryId);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                return entity;
            }
            throw new ArgumentException("The key must be provided in the format 'ArticleID-CategoryID'.");
        }

        public async Task<IEnumerable<ArticleCategory>> DeleteByArticleID(string key)
        {
            int articleId = int.Parse(key);

            var entities = _dbSet.Where(ac => ac.ArticleID == articleId);
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();

            }
            return entities;
        }

        public async Task<IEnumerable<ArticleCategory>> DeleteByCategoryID(string key)
        {

            int categoryId = int.Parse(key);

            var entities = _dbSet.Where(ac => ac.CategoryID == categoryId);
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
            return entities;
        }

        public async Task<ArticleCategory> Get(string key, string value)
        {
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id"))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(ArticleCategory), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<ArticleCategory, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);
            return result;
        }

        public async Task<IEnumerable<ArticleCategory>> GetAll(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                var result1 = await _dbSet.ToListAsync();
                return result1;

            }

            var propertyInfo = typeof(ArticleCategory).GetProperty(key);
            if (propertyInfo == null)
            {
                throw new ColumnNotExistException(key, "ArticleCategory");
            }

            var parameter = Expression.Parameter(typeof(ArticleCategory), "e");
            var property = Expression.Property(parameter, key);

            Expression<Func<ArticleCategory, bool>> lambda;

            var constantValue = Expression.Constant(int.Parse(value));
            var equalExpression = Expression.Equal(property, constantValue);
            lambda = Expression.Lambda<Func<ArticleCategory, bool>>(equalExpression, parameter);

            var result = await _dbSet.Where(lambda).ToListAsync();


            return result;
        }
        [ExcludeFromCodeCoverage]
        public async Task<ArticleCategory> Update(ArticleCategory item, string key)
        {
            throw new NotImplementedException();
        }
    }
}
