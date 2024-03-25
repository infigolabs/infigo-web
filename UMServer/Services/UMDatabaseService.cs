using System.Collections.Generic;

namespace UMServer.Services
{
	public class UMDatabaseService : SQLiteDatabaseService
	{		
		public override string DatabaseFile => "user_data_store.db";

		public override void Initialize()
		{
			base.Initialize();

			List<List<object>> planData = new List<List<object>>
			{
				new List<object>() { 0, 14, 0, "14 days subscription" },
				new List<object>() { 101, 90, 15, "3 months subscription" },
				new List<object>() { 102, 180, 25, "6 months subscription" },
				new List<object>() { 103, 365, 50, "1 year subscription" }
			};

			InsertRows(Constants.PLANS_TABLE, planData);
		}

		public override void CreateTables()
		{
			CreateTable(Constants.USERS_TABLE, "user_id TEXT PRIMARY KEY NOT NULL, plan_id INTEGER, license_key TEXT, email TEXT, name Text, product_version TEXT," +
				"subscription_status TEXT, subscription_start NUMERIC, subscription_end NUMERIC, active BOOLEAN, is_expired BOOLEAN, is_device_actived BOOLEAN");
			CreateTable(Constants.PREMIUM_USERS_TABLE, "license_key TEXT, plan_id INTEGER");
			CreateTable(Constants.USER_DETAILS_TABLE, "user_id TEXT, os TEXT, os_version TEXT, country TEXT");
			CreateTable(Constants.PLANS_TABLE, "plan_id INTEGER PRIMARY KEY NOT NULL, plan_length INTEGER, plan_price REAL, plan_description TEXT");
		}
	}
}
