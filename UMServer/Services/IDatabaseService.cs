using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UMServer.Services
{
	public interface IDatabaseService 
	{
		void Initialize();
		bool ExistsRowWhere(string table, string whereColumn, string equalsValue);
		List<string> GetRowWhere(string table, List<string> columns, string whereColumn, string equalsValue);
		bool InsertRow(string table, List<object> values, bool replaceIfExists = true);
		bool InsertRows(string table, List<List<object>> rowsOfValues, bool replaceIfExists = true);

        
    }
}