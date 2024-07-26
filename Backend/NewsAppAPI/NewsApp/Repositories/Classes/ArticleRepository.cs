using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using static NewsApp.Models.Enum;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection.Metadata;
using NewsApp.Repositories.Interfaces;

namespace NewsApp.Repositories.Classes
{
    public class ArticleRepository : IRepository<string, Article, string>
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<Article> _dbSet;

        public ArticleRepository(NewsAppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<Article>();
        }

        public async Task<Article> Add(Article item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Article> Delete(string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<Article> Get(string key, string value)
        {
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id") && !(key.ToLower().Contains("hash")))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(Article), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<Article, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);

            return result;

        }

        public async Task<IEnumerable<Article>> GetAll(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                var result1 = await _dbSet.ToListAsync();

                return result1;

            }

            var propertyInfo = typeof(Article).GetProperty(key);
            if (propertyInfo == null)
            {
                throw new ColumnNotExistException(key, "Article");
            }

            var parameter = Expression.Parameter(typeof(Article), "e");
            var property = Expression.Property(parameter, key);

            Expression<Func<Article, bool>> lambda;

            if (key.ToLower().Contains("id"))
            {
                var constantValue = Expression.Constant(int.Parse(value));
                var equalExpression = Expression.Equal(property, constantValue);
                lambda = Expression.Lambda<Func<Article, bool>>(equalExpression, parameter);
            }
            else if (key.ToLower().Contains("status"))
            {
                if (System.Enum.TryParse<ArticleStatus>(value, true, out var statusEnum))
                {
                    var constantValue = Expression.Constant(statusEnum);
                    var equalExpression = Expression.Equal(property, constantValue);
                    lambda = Expression.Lambda<Func<Article, bool>>(equalExpression, parameter);
                }
                else
                {
                    throw new ArgumentException($"Invalid status value: {value}", nameof(value));
                }
            }
            else
            {
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var propertyToLower = Expression.Call(property, toLowerMethod);
                var constantValue = Expression.Constant(value.ToLower());
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(propertyToLower, containsMethod, constantValue);
                lambda = Expression.Lambda<Func<Article, bool>>(containsExpression, parameter);
            }

            var result = await _dbSet.Where(lambda).ToListAsync();

            return result;

        }

        public async Task<Article> Update(Article item, string key)
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
