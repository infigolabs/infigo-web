using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UMServer.Common;

namespace UMServer.Services
{
    public class DbOperations<TEntity> : IDbOperations<TEntity> where TEntity : class
    {
        private readonly ApplicationDBContext _context;
        private DbSet<TEntity> _dbSet;


        public DbOperations(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified; // Explicitly set the state to Modified
            await _context.SaveChangesAsync();
            return true;
        }

        public List<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            // return _dbSet.ToList();
            return await _dbSet.ToListAsync();

        }
        public async Task<List<TEntity>> GetAllWithEagerLoading(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }
    }
}
