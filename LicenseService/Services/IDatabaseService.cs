using System.Collections.Generic;

namespace LicenseService.Services
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