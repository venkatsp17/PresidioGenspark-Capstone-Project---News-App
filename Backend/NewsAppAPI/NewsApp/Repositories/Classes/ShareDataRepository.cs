using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NewsApp.Repositories.Classes
{
    public class ShareDataRepository : IRepository<string, ShareData, string>
    {
        protected readonly NewsAppDBContext _context;
        private readonly DbSet<ShareData> _dbSet;


        public ShareDataRepository(NewsAppDBContext context)
        {
            _context = context;

            _dbSet = _context.Set<ShareData>();
        }

        public async Task<ShareData> Add(ShareData item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<ShareData> Delete(string key)
        {
            var entity = await _dbSet.FindAsync(int.Parse(key));
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<ShareData> Get(string key, string value)
        {
            var constant = Expression.Constant(value);
            if (key.ToLower().Contains("id"))
            {
                constant = Expression.Constant(int.Parse(value));
            }
            var parameter = Expression.Parameter(typeof(ShareData), "e");
            var property = Expression.Property(parameter, key);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<ShareData, bool>>(equal, parameter);

            var result = await _dbSet.FirstOrDefaultAsync(lambda);

            return result;

        }


        public async Task<IEnumerable<ShareData>> GetAll(string key, string value)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
            {
                var result1 = await _dbSet.ToListAsync();

                return result1;

            }

            var propertyInfo = typeof(ShareData).GetProperty(key);
            if (propertyInfo == null)
            {
                throw new ColumnNotExistException(key, "ShareData");
            }

            var parameter = Expression.Parameter(typeof(ShareData), "e");
            var property = Expression.Property(parameter, key);

            Expression<Func<ShareData, bool>> lambda;
            var constantValue = Expression.Constant(value);
            if (key.ToLower().Contains("id"))
            {
                constantValue = Expression.Constant(int.Parse(value));
            }
           
            var equalExpression = Expression.Equal(property, constantValue);
            lambda = Expression.Lambda<Func<ShareData, bool>>(equalExpression, parameter);

            var result = await _dbSet.Where(lambda).ToListAsync();


            return result;

        }

        public async Task<ShareData> Update(ShareData item, string key)
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
