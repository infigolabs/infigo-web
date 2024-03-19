using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace UMServer.Services
{
    public interface IDbOperations<TEntity> where TEntity : class
    {
        Task<TEntity> Insert(TEntity entity);
        Task<bool> Update(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        List<TEntity> GetAll();
        Task<List<TEntity>> GetAllWithEagerLoading(params Expression<Func<TEntity, object>>[] includes);

    }
}
