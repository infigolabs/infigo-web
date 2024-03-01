using Common.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UMServer.Services
{
	public class AccountService : IAccountService
	{
		private readonly IDatabaseService mDatabaseService;

		public AccountService(IDatabaseService databaseService)
		{
			mDatabaseService = databaseService;
		}

		/// <summary>
		/// Get account details for user id.
		/// </summary>
		/// <param name="userid"></param>
		/// <returns></returns>
		public ApiResult GetAccountInfo(string userid)
		{
			if (string.IsNullOrWhiteSpace(userid))
				return new ApiResult($"Argument {userid} is null");

			var account = new AccountMetadata();
			var result = new ApiResult();
			try
			{
				List<string> columns = new List<string>() { "user_id", "email", "name", "plan_id", "subscription_status", "subscription_start", "subscription_end", "active", "is_expired", "is_device_actived" };
				var row = mDatabaseService.GetRowWhere(Constants.USERS_TABLE, columns, "user_id", userid);

				if (row == null || row.Count == 0)
				{
					result.StatusCode = (int)StatusCodes.AccountNotFound;
					result.Error = $"User {userid} is not found";
				}
				else
				{
					var planData = mDatabaseService.GetRowWhere(Constants.PLANS_TABLE, new List<string>() { "plan_description" }, "plan_id", row[3]);
					
					UserMetadata userMetadata = new() { UserId = userid, Email = row[1], Name = row[2] };
					PlanMetadata planMetadata = new() { PlanId = Convert.ToInt32(row[3]), PlanDescription = planData[0] };
					SubscriptionMetadata subscriptionMetadata = new();
					subscriptionMetadata.SubscriptionStatus = row[4];
					subscriptionMetadata.SubscriptionStart = Convert.ToUInt64(row[5]);
					subscriptionMetadata.SubscriptionEnd = Convert.ToUInt64(row[6]);

					account.User = userMetadata;
					account.Plan = planMetadata;
					account.Subscription = subscriptionMetadata;
					account.IsActive = (row[7] == "1" ? true : false);
					account.IsExpired = (row[8] == "1" ? true : false);
					account.IsDeviceActivated = (row[8] == "1" ? true : false);
					result.Data = JsonConvert.SerializeObject(account);
				}
			}
			catch (Exception ex)
			{
				result.StatusCode = (int)StatusCodes.FetchAccountErr;
				result.Error = $"Error while fetching information {ex.Message}";
			}
			
			return result;

			// If account does not exist, return null and UI will offer user to start the trial.

			// If exist with subscription "trial" regardless of subsciption expiry, return details and UI will offer this user to buy the license.

			// If account exist with subscription "premium", return details and UI will not show buy license banner. 
		}

		/// <summary>
		/// Start the trial for new user.
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="planId"></param>
		/// <returns></returns>
		public ApiResult StartTrial(TrialMetadata metadata)
		{
			
			if (metadata == null)
				return new ApiResult($"{nameof(StartTrial)}: {nameof(metadata)} is null");

			if (string.IsNullOrWhiteSpace(metadata.UserId))
				return new ApiResult($"{nameof(StartTrial)}: {nameof(metadata.UserId)} is null or empty");

			if (metadata.PlanId < 0)
				return new ApiResult($"{nameof(StartTrial)}: Invalid planid {nameof(metadata.PlanId)}");

			ApiResult result = new ApiResult();

			try
			{

				if (CreateTrialPlan(metadata))
				{
					return GetAccountInfo(metadata.UserId);
				}
				else
				{
					result.StatusCode = (int)StatusCodes.StartTrialFailed;
					result.Error = "Failed to start the trial";
				}
			}
			catch(Exception ex)
			{
				result.StatusCode = (int)StatusCodes.StartTrialErr;
				result.Error = $"{nameof(StartTrial)}: Error while starting trial. {ex.Message}";
			}

			return result;
		}		

		/// <summary>
		/// Register user when purchase is made.
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="planId"></param>
		/// <param name="email"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public ApiResult Register(RegistrationMetadata metadata)
		{
			if (metadata == null)
				return new ApiResult($"{nameof(Register)}: {nameof(metadata)} is null");

			if (string.IsNullOrWhiteSpace(metadata.UserId))
				return new ApiResult($"{nameof(Register)}: {nameof(metadata.UserId)} is null or empty");

			if (string.IsNullOrWhiteSpace(metadata.Email))
				return new ApiResult($"{nameof(Register)}: {nameof(metadata.Email)} is null or empty");

			if (metadata.PlanId < 0)
				return new ApiResult($"{nameof(Register)}: Invalid planid {nameof(metadata.PlanId)}");


			// Generate license key and send it to user email.
			// If user is already on valid trial, only update license_key, email and name fields.
			// Store a record in premium_users table for generated license_key and purchased planid.

			// If paid subscription is not activated, trial period can still be used as long as it is valid.

			return null;
		}

		/// <summary>
		/// Validate the purchase and start the subscription
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="licenseKey"></param>
		/// <returns></returns>
		public ApiResult Validate(string userid, string licenseKey)
		{
			if (string.IsNullOrWhiteSpace(userid))
				return null;

			if (string.IsNullOrWhiteSpace(licenseKey))
				return null;

			// Fetch user details by userid.
			// Update the license_key field.
			// Generate new subscription start and end dates.
			// Update subscription_status to "paid"
			// Update plan_id to new one for this license_key from premium_users table.
			return null;
		}

		private bool CreateTrialPlan(TrialMetadata trialMetadata)
		{
			List<string> planValues = mDatabaseService.GetRowWhere(Constants.PLANS_TABLE, new List<string>() { "plan_length" }, "plan_id", trialMetadata.PlanId.ToString());
			// user_id | plan_id | license_key | email | name | product_version | subscription_status | subscription_start | subscription_end | active | is_expired | is_device_active

			// Generate subscription start and end date
			// Set active to true and is_expired to false
			// Subscription_status to "trial".
			// update user_details table.

			ulong planLength = Convert.ToUInt64(planValues[0]);
			List<object> values = new List<object>();
			values.Add(trialMetadata.UserId);
			values.Add(trialMetadata.PlanId);
			values.Add(string.Empty);
			values.Add(string.Empty);
			values.Add(string.Empty);
			values.Add(string.Empty);
			values.Add(SubscriptionType.trial.ToString());

			ulong currentTime = TimeUtils.Now();
			values.Add(currentTime);
			values.Add((currentTime + planLength * 24 * 60 * 60));
			values.Add(true);
			values.Add(false);
			values.Add(true);
			if (mDatabaseService.InsertRow(Constants.USERS_TABLE, values))
			{
				mDatabaseService.InsertRow(Constants.USER_DETAILS_TABLE,
					new List<object>() { trialMetadata.UserId, trialMetadata.OS, trialMetadata.OSVersion, trialMetadata.Country });
				return true;
			}

			return false;
		}
	}

	enum SubscriptionType
	{
		none,
		trial,
		paid,
		expired
	}

	enum StatusCodes
	{
		Success,
		AccountNotFound = 11,
		FetchAccountErr = 10,
		StartTrialErr = 20,
	    StartTrialFailed = 21
	}
}
