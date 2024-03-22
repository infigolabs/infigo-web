using Common.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMServer.Models;

namespace UMServer.Services
{
	public class AccountService : IAccountService
	{
		private readonly IDatabaseService mDatabaseService;
        private readonly IDbOperations<User> _user;
        private readonly IDbOperations<Plan> _plan;
        private readonly IDbOperations<UserDetail> _userdetail;
        private readonly IDbOperations<PremiumUser> _premiumuser;
        public AccountService(IDatabaseService databaseService, IDbOperations<User> user, IDbOperations<Plan> plan, IDbOperations<UserDetail> userdetail, IDbOperations<PremiumUser> premiumuser)
		{
			mDatabaseService = databaseService;
            _user = user;
            _plan = plan;
            _userdetail = userdetail;
            _premiumuser = premiumuser;
        }

        /// <summary>
        /// Get account details for user id.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
		public async Task<ApiResult> GetAccountInfos(string userid) 
		{
            if (string.IsNullOrWhiteSpace(userid))
                return new ApiResult($"Argument {userid} is null");

            var result = new ApiResult();
            try
            {
                var users = await _user.GetAllAsync();
                var user = users.Where(x => x.UserId == int.Parse(userid));        
                if (user.Count() == 0)
                {
                    result.Error = $"User {userid} is not found";
                    result.StatusCode = (int)StatusCodes.AccountNotFound;
                    return result;
                }
                else
                {
                    result.StatusCode = (int)StatusCodes.Success;
                    result.Data = JsonConvert.SerializeObject(user.ToList()[0]);
                    return result;
                }
            }
            catch (Exception ex) 
            {
                result.Error = $"Error while fetching information {ex.Message}";
                result.StatusCode = (int)StatusCodes.FetchAccountErr;
                return result;
            }
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


		public async Task<ApiResult> StartTrials(TrialMetadata metadata)
		{
            if (metadata == null)
                return new ApiResult($"{nameof(StartTrials)}: {nameof(metadata)} is null");

            if (string.IsNullOrWhiteSpace(metadata.UserId))
                return new ApiResult($"{nameof(StartTrials)}: {nameof(metadata.UserId)} is null or empty");

            if (metadata.PlanId < 0)
                return new ApiResult($"{nameof(StartTrials)}: Invalid planid {nameof(metadata.PlanId)}");

            ApiResult result = new ApiResult();

            try
            {

                if (await CreateTrialPlans(metadata))
                {
                    return await GetAccountInfos(metadata.UserId);
                }
                else
                {
                    result.StatusCode = (int)StatusCodes.StartTrialFailed;
                    result.Error = "Failed to start the trial";
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = (int)StatusCodes.StartTrialErr;
                result.Error = $"{nameof(StartTrials)}: Error while starting trial. {ex.Message}";
            }

            return result;
        }

		public async Task<ApiResult> Register(RegistrationMetadata metadata)
		{
            ApiResult result = new ApiResult();
            if (metadata == null)
				return new ApiResult($"{nameof(Register)}: {nameof(metadata)} is null");

			if (string.IsNullOrWhiteSpace(metadata.UserId))
				return new ApiResult($"{nameof(Register)}: {nameof(metadata.UserId)} is null or empty");

			if (string.IsNullOrWhiteSpace(metadata.Email))
				return new ApiResult($"{nameof(Register)}: {nameof(metadata.Email)} is null or empty");

			if (metadata.PlanId < 0)
				return new ApiResult($"{nameof(Register)}: Invalid planid {nameof(metadata.PlanId)}");

            try
            {
                var users = await _user.GetAllAsync();
                var user = users.Where(x => x.UserId == int.Parse(metadata.UserId)).ToList();
                // Generate license key and send it to user email.
                var licensekey = GenerateLicenseKey(metadata.UserId);
                // If user is already on valid trial, only update license_key, email and name fields.
                if (user.Count() > 0)
                {
                    var currentuser = user[0];
                    currentuser.LicenseKey = licensekey;
                    currentuser.Email = metadata.Email;

                    var userdetails = await _userdetail.GetAllAsync();
                    var userdetail = userdetails.Where(x => x.UserId == int.Parse(metadata.UserId)).First();
                    userdetail.Name = metadata.Name;

                    await _user.Update(currentuser);
                    await _userdetail.Update(userdetail);
                }
                else 
                {
                    result.Error = $"User {metadata.UserId} is not found";
                    result.StatusCode = (int)StatusCodes.AccountNotFound;
                    return result;
                }
                // Store a record in premium_users table for generated license_key and purchased planid.
                var premiumUser = new PremiumUser
                {
                    LicenseKey = licensekey,
                    PlanId = metadata.PlanId
                };

                await _premiumuser.Insert(premiumUser);
                // If paid subscription is not activated, trial period can still be used as long as it is valid.
                result.StatusCode = (int)StatusCodes.Success;

                return result;
            }
            catch (Exception ex) 
            {
                result.Error=ex.Message;
                result.StatusCode = (int)StatusCodes.RegisterFailed;
                return result;
            }
		}

		/// <summary>
		/// Validate the purchase and start the subscription
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="licenseKey"></param>
		/// <returns></returns>
		public async Task<ApiResult> Validate(string userid, string licenseKey)
		{
            var result = new ApiResult();
            if (string.IsNullOrWhiteSpace(userid))
				return null;

			if (string.IsNullOrWhiteSpace(licenseKey))
				return null;
            try
            {
                // Fetch user details by userid.
                var users = await _user.GetAllAsync();
                if (users.Count() == 0)
                {
                    result.Error = $"User {userid} is not found";
                    result.StatusCode = (int)StatusCodes.AccountNotFound;
                    return result;
                }
                else
                {
                    var user = users.Where(x => x.UserId == int.Parse(userid)).ToList()[0];

                    // Update the license_key field.
                    user.LicenseKey = licenseKey;

                    // Generate new subscription start and end dates.
                    var plain_ids = await _premiumuser.GetAllAsync();
                    var plain_id = plain_ids.Where(x => x.LicenseKey == licenseKey).Select(x => x.PlanId).ToList()[0];
                    var plans = await _plan.GetAllAsync();
                    var plan_length = plans.Where(x => x.PlanId == plain_id).Select(x => x.PlanLength).ToList()[0];

                    // Update subscription_status to "paid"
                    user.SubscriptionStatus = nameof(SubscriptionType.paid);

                    // Update plan_id to new one for this license_key from premium_users table.
                    user.PlanId = plain_id;
                    await _user.Update(user);
                    result.Data = JsonConvert.SerializeObject(user);
                    result.StatusCode = (int)StatusCodes.Success;
                    return result;
                }
            }
            catch (Exception ex) 
            {
                result.Error = ex.Message;
                result.StatusCode = (int)StatusCodes.ValidateFailed;
                return result;
            }
		}


		private async Task<bool> CreateTrialPlans(TrialMetadata trialMetadata)
		{
			var plans= await _plan.GetAllAsync();
			List<int> planValues = plans.Where(x => x.PlanId == trialMetadata.PlanId).Select(x=>x.PlanLength).ToList();

			ulong planLength = Convert.ToUInt64(planValues[0]);
            User user = new User
            {
                PlanId = 1,
                LicenseKey = "ABC123",
                Email = "example@example.com",
                ProductVersion = "1.0",
                SubscriptionStatus = "Active",
                SubscriptionStart = DateTime.Now,
                SubscriptionEnd = DateTime.Now.AddDays(planLength),
                Active = true,
                IsExpired = false,
                IsDeviceActive = true
            };
			var userentity=await _user.Insert(user);

            UserDetail userDetails = new UserDetail
            {
                UserId = userentity.UserId,
                Name = "John Doe",
                OS = "Windows",
                OSVersion = "10",
                Country = "USA"
            };
			var userdetailentity= await _userdetail.Insert(userDetails);
           			
            return true;
        }

        private string GenerateLicenseKey(string userid)
        {
            // Generate a unique GUID
            Guid guid = Guid.NewGuid();

            // Convert GUID to a string and remove hyphens
            string guidString = guid.ToString("N");

            // Concatenate the product name and the GUID
            string licenseKey = $"{userid}-{guidString}";

            return licenseKey;
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
	    StartTrialFailed = 21,
        RegisterFailed=30,
        ValidateFailed = 30
    }
}
