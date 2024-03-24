using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UMServer.Services
{
	public abstract class SQLiteDatabaseService : ISQLiteDatabaseService
	{
		public abstract string DatabaseFile { get; }
		protected SqliteConnection mConnection;
		private string mDatabaseFilePath;

		public virtual void Initialize()
		{
			if (string.IsNullOrWhiteSpace(DatabaseFile))
				throw new Exception("Invalid database file");

			if (!Directory.Exists(Configuration.DatabaseLocation))
			{
				Directory.CreateDirectory(Configuration.DatabaseLocation);
			}

			mDatabaseFilePath = Path.Combine(Configuration.DatabaseLocation, DatabaseFile);

			mConnection = new SqliteConnection($"Data Source={mDatabaseFilePath}");

			try
			{
				mConnection.Open();

				CreateTables();
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"{nameof(SQLiteDatabaseService)}, Open, Error occurred {ex.Message}");
				throw;
			}
		}

		protected void CreateTable(string table, string columns)
		{
			if (TableExist(table))
			{
				return;
			}

			try
			{
				using (SqliteCommand command = mConnection.CreateCommand())
				{
					command.CommandText = $"CREATE TABLE {table} ({columns})";
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"{nameof(SQLiteDatabaseService)}, {nameof(CreateTable)}, Error occurred for {table} {ex.Message}");
				throw;
			}
		}

		protected bool TableExist(string table)
		{
			using (SqliteCommand command = mConnection.CreateCommand())
			{
				command.CommandText = $"SELECT count(*) FROM sqlite_master WHERE name='{table}'";
				int count = Convert.ToInt32(command.ExecuteScalar());
				return count > 0;
			}
		}

		protected void PurgeTable(string table)
		{
			try
			{
				using (SqliteCommand command = mConnection.CreateCommand())
				{
					command.CommandText = $"DELETE FROM {table}";
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"{nameof(SQLiteDatabaseService)}, {nameof(PurgeTable)}, Error occurred for {table} {ex.Message}");
				throw;
			}
		}

		internal SqliteDataReader GetRowWhereDataReader(string table, List<string> columns, string whereColumn, string equalsValue)
		{
			try
			{
				SqliteCommand command = mConnection.CreateCommand();

				command.CommandText = "SELECT " + string.Join(",", columns.ToArray()) + " FROM " + table + " WHERE " + whereColumn + " = @equalsValue LIMIT 1";
				command.Parameters.AddWithValue("@equalsValue", equalsValue);
				var reader = command.ExecuteReader();
				return reader;
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"{nameof(SQLiteDatabaseService)}, {nameof(GetRowWhereDataReader)}, Error occurred for {table} {ex.Message}");
			}

			return null;
		}

		public List<string> GetRowWhere(string table, List<string> columns, string whereColumn, string equalsValue)
		{
			try
			{
				using (var reader = GetRowWhereDataReader(table, columns, whereColumn, equalsValue))
				{
					if (reader == null)
					{
						return null;
					}

					var result = new List<List<string>>();
					while (reader.Read())
					{
						var row = new List<string>();
						for (int i = 0; i < columns.Count; i++)
						{
							row.Add(reader.GetString(i));
						}

						return row;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"{nameof(SQLiteDatabaseService)}, {nameof(GetRowWhere)}, Error occurred for {table} {ex.Message}");
			}

			return null;
		}

		public bool ExistsRowWhere(string table, string whereColumn, string equalsValue)
		{
			try
			{
				using (SqliteCommand command = mConnection.CreateCommand())
				{
					command.CommandText = "SELECT count(*) FROM " + table + " WHERE " + whereColumn + " = @equalsValue LIMIT 1";
					command.Parameters.AddWithValue("@equalsValue", equalsValue);
					return (long)command.ExecuteScalar() > 0;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine($"{nameof(SQLiteDatabaseService)}, {nameof(ExistsRowWhere)}, Error occurred for {table} {ex.Message}");
			}

			return false;
		}

		public bool InsertRow(string table, List<object> values, bool replaceIfExists = true)
		{
			return InsertRows(table, new List<List<object>> { values }, replaceIfExists);
		}

		public bool InsertRows(string table, List<List<object>> rowsOfValues, bool replaceIfExists = true)
		{
			try
			{
				if (rowsOfValues.Count == 0)
				{
					return true;
				}

				using (SqliteCommand command = mConnection.CreateCommand())
				{
					command.CommandText = "INSERT OR " + (replaceIfExists ? "REPLACE" : "IGNORE") + " INTO " + table + " VALUES ";
					int count = 0;
					foreach (var values in rowsOfValues)
					{
						command.CommandText += "(";
						foreach (var value in values)
						{
							count++;
							string parameterName = "@value" + count.ToString(CultureInfo.InvariantCulture);
							command.CommandText += parameterName + ",";
							command.Parameters.AddWithValue(parameterName, value);
						}
						command.CommandText = command.CommandText.Remove(command.CommandText.Length - 1) + "),";
					}
					command.CommandText = command.CommandText.Remove(command.CommandText.Length - 1);
					command.ExecuteNonQuery();
				}
			}
			catch
			{
				throw;
			}

			return true;
		}

		public bool DeleteRow(string table, string whereColumn, string equalsValue)
		{
			try
			{
				using (SqliteCommand command = mConnection.CreateCommand())
				{
					command.CommandText = "DELETE FROM " + table + " WHERE " + whereColumn + " = @equalsValue";
					command.Parameters.AddWithValue("@equalsValue", equalsValue);
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ee)
			{
				return false;
			}

			return true;
		}

		public abstract void CreateTables();
	}
}
