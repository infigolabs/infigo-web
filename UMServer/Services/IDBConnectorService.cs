using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using UMServer.Services;

namespace UMServer
{
	public interface IDBConnectorService<TEntity>: IDbOperations<TEntity> where TEntity : class
	{
	}
}