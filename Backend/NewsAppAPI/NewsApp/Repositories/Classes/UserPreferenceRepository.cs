using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Migrations;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using static NewsApp.Models.Enum;

namespace NewsApp.Repositories.Classes
{
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<UserPreference> _dbSet;

        public UserPreferenceRepository(NewsAppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<UserPreference>();
        }

        public async Task<UserPreference> Add(UserPreference item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<UserPreference> Delete(string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<UserPreference> Get(string key, string value)
        {
            
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id"))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(UserPreference), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<UserPreference, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);
            return result;
    

        }

        public async Task<IEnumerable<UserPreference>> DeleteByUserID(string key)
        {
            int userid = int.Parse(key);

            var entities = _dbSet.Where(ac => ac.UserID == userid);
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
                return entities;

            }
            return null;
        }

        public async Task<IEnumerable<UserPreference>> DeleteByCategoryID(string key)
        {

            int categoryId = int.Parse(key);

            var entities = _dbSet.Where(ac => ac.CategoryID == categoryId);
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
                return entities;
            }
            return null;
        }



        public async Task<IEnumerable<UserPreference>> GetAll(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                var result1 = await _dbSet.ToListAsync();
                return result1;

            }

            var propertyInfo = typeof(UserPreference).GetProperty(key);
            if (propertyInfo == null)
            {
                throw new ColumnNotExistException(key, "UserPreference");
            }

            var parameter = Expression.Parameter(typeof(UserPreference), "e");
            var property = Expression.Property(parameter, key);

            Expression<Func<UserPreference, bool>> lambda;

            var constantValue = Expression.Constant(int.Parse(value));
            var equalExpression = Expression.Equal(property, constantValue);
            lambda = Expression.Lambda<Func<UserPreference, bool>>(equalExpression, parameter);

            var result = await _dbSet.Where(lambda).ToListAsync();


            return result;
        }
        [ExcludeFromCodeCoverage]
        public async Task<UserPreference> Update(UserPreference item, string key)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoryPreferenceDto>> LikedDiskedAriclesORder()
        {
            var categoryPreferences = await _dbSet
               .GroupBy(up => up.Category.Name)
               .Select(g => new CategoryPreferenceDto
               {
                   CategoryName = g.Key,
                   Likes = g.Count(up => up.preference == Preference.Like),
                   Dislikes = g.Count(up => up.preference == Preference.DisLike)
               })
               .OrderByDescending(cp => cp.Likes)
               .ToListAsync(); 

            return categoryPreferences;
        }
    }
}
