using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using UMServer.Models;

namespace UMServer.Services
{
	public class DBConnectorService<TEntity> : IDbOperations<TEntity> where TEntity : class
	{
		private readonly ISQLiteDatabaseService mSQLiteDatabaseService;

		public DBConnectorService(ISQLiteDatabaseService sqliteDatabaseService)
        {
			mSQLiteDatabaseService = sqliteDatabaseService;
		}
        public List<TEntity> GetAll()
		{
			return null;
		}

		public Task<List<TEntity>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<List<TEntity>> GetAllWithEagerLoading(params Expression<Func<TEntity, object>>[] includes)
		{
			throw new NotImplementedException();
		}

		public Task<TEntity> Insert(TEntity entity)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Update(TEntity entity)
		{
			throw new NotImplementedException();
		}
	}
}
