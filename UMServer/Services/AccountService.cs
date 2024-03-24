using Common.Data;
using Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using UMServer.Common;
using UMServer.Models;

namespace UMServer.Services
{
	public class AccountService : IAccountService
	{
		private readonly ApplicationDBContext mDBContext;

		public AccountService(ApplicationDBContext dbContext)
		{
			mDBContext = dbContext;
		}

		public async Task<ApiResult> GetAccountInfo(string userid)
		{
			if (string.IsNullOrWhiteSpace(userid))
				return new ApiResult($"Argument {userid} is null");

			var result = new ApiResult();
			try
			{
				AccountMetadata metadata = null;
				var account = mDBContext.Accounts.FirstOrDefault(x => x.AccountId == userid);
				if (account == null)
				{
					var trialPlan = mDBContext.Plans.FirstOrDefault(x => x.PlanId == 0);
					
					metadata = new AccountMetadata()
					{
						Plan = new PlanMetadata()
						{
							PlanId = trialPlan.PlanId,
							PlanLength = trialPlan.PlanLength,
							PlanDescription = trialPlan.PlanDescription
						},
						Subscription = new SubscriptionMetadata()
						{
							SubscriptionStatus = SubscriptionType.None.ToString()
						}
					};
				}
				else
				{
					metadata = ToAccountMetadata(account);
				}

				result.StatusCode = (int)StatusCodes.Success;
				result.Data = JsonConvert.SerializeObject(metadata);
				return result;
			}
			catch (Exception ex)
			{
				result.Error = $"Error while fetching information {ex.Message}";
				result.StatusCode = (int)StatusCodes.FetchAccountErr;
				return result;
			}
		}

		public async Task<ApiResult> StartTrial(TrialMetadata metadata)
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
				ThrowIfAccountExist(metadata.UserId);

				var acct = await CreateTrialPlan(metadata);
				result.StatusCode = (int)StatusCodes.Success;
				result.Data = JsonConvert.SerializeObject(ToAccountMetadata(acct));
			}
			catch (Exception ex)
			{
				result.StatusCode = (int)StatusCodes.StartTrialErr;
				result.Error = $"{nameof(StartTrial)}: Error while starting trial. {ex.Message}";
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
				var account = ThrowIfAccountDoesNotExist(metadata.UserId);

				// If user is already on valid trial, only update license_key, email and name fields.
				// Generate license key and send it to user email.
				var licensekey = GenerateLicenseKey(metadata.UserId);

				account.LicenseKey = licensekey;
				account.Email = metadata.Email;
			
				mDBContext.Accounts.Update(account);

				// Store a record in premium_users table for generated license_key and purchased planid.
				var premiumUser = new PremiumUser
				{
					AccountId = account.AccountId,
					PlanId = metadata.PlanId
				};

				mDBContext.PremiumUsers.Add(premiumUser);

				await mDBContext.SaveChangesAsync();

				// TODO: Send email to user about license key.

				// If paid subscription is not activated, trial period can still be used as long as it is valid.
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.StatusCode = (int)StatusCodes.RegisterFailed;
			}

			return result;
		}

		public async Task<ApiResult> Activate(string userid, string licenseKey)
		{
			ApiResult result = new ApiResult();
			try
			{
				if (string.IsNullOrWhiteSpace(userid))
					throw new Exception("Invalid account id");

				if (string.IsNullOrWhiteSpace(licenseKey))
					throw new Exception("Invalid license key");

				var account = ThrowIfAccountDoesNotExist(userid);
				var premiumAccount = mDBContext.PremiumUsers.FirstOrDefault(acct => acct.AccountId == userid);
				if (premiumAccount == null)
					throw new Exception("No subscription found for this account");

				if (account.LicenseKey != licenseKey)
					throw new Exception("License key is invalid");

				var plan = mDBContext.Plans.FirstOrDefault(x => x.PlanId == premiumAccount.PlanId);

				if (plan == null)
					throw new Exception("No valid plan found for account");

				var otherAccount = mDBContext.Accounts.FirstOrDefault(x => x.LicenseKey == licenseKey && x.AccountId != userid);

				if (otherAccount != null)
					throw new Exception("License key is already activated on another device.");

				// Update the license_key field.
				account.LicenseKey = licenseKey;

				account.SubscriptionStart = TimeUtils.Now();
				account.SubscriptionEnd = TimeUtils.AddDays(account.SubscriptionStart, plan.PlanLength);

				// Update subscription_status to "paid"
				account.SubscriptionStatus = SubscriptionType.Paid.ToString();

				// Update plan_id to new one for this license_key from premium_users table.
				account.PlanId = premiumAccount.PlanId;

				mDBContext.Accounts.Update(account);
				await mDBContext.SaveChangesAsync();

				result.Data = JsonConvert.SerializeObject(ToAccountMetadata(account));
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.StatusCode = (int)StatusCodes.ValidateFailed;
			}

			return result;
		}

		private async Task<Account> CreateTrialPlan(TrialMetadata metadata)
		{
			var plan = mDBContext.Plans.FirstOrDefault(x => x.PlanId == metadata.PlanId);

			if (plan == null)
				throw new Exception("Plan not found");

			Account account = new Account
			{
				AccountId = metadata.UserId,
				PlanId = plan.PlanId,
				SubscriptionStatus = SubscriptionType.Trial.ToString(),
				SubscriptionStart = TimeUtils.Now(),
				SubscriptionEnd = TimeUtils.AddDays(TimeUtils.Now(), plan.PlanLength),
				Active = true,
				IsExpired = false,
				IsDeviceActive = true,
				OS = metadata.OS,
				OSVersion = metadata.OSVersion,
				Country = metadata.Country
			};
			
			mDBContext.Accounts.Add(account);
			await mDBContext.SaveChangesAsync();

			return account;
		}

		public async Task<ApiResult> Deactivate(string userid)
		{
			ApiResult result = new ApiResult();
			try
			{
				var account = ThrowIfAccountDoesNotExist(userid);
				account.IsDeviceActive = false;
				result.Data = JsonConvert.SerializeObject(ToAccountMetadata(account));
				return result;
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.StatusCode = (int)StatusCodes.ValidateFailed;
			}

			return result;
		}

		private Account ThrowIfAccountExist(string userid)
		{
			var account = mDBContext.Accounts.FirstOrDefault(x => x.AccountId == userid);
			if (account != null)
				throw new Exception($"Account {userid} is already registered");

			return account;
		}

		private Account ThrowIfAccountDoesNotExist(string userid)
		{
			var account = mDBContext.Accounts.FirstOrDefault(x => x.AccountId == userid);
			if (account == null)
				throw new Exception($"Account {userid} not found");

			return account;
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

		private AccountMetadata ToAccountMetadata(Account account)
		{
			var plan = mDBContext.Plans.FirstOrDefault(p => p.PlanId == account.PlanId);
			return new AccountMetadata()
			{
				IsActive = account.Active,
				IsExpired = account.IsExpired,
				IsDeviceActivated = account.IsDeviceActive,
				User = new UserMetadata() { UserId = account.AccountId, Email = account.Email},
				Plan = new PlanMetadata()
				{
					PlanId = plan.PlanId,
					PlanLength = plan.PlanLength,
					PlanDescription = plan.PlanDescription,
				},
				Subscription = new SubscriptionMetadata()
				{
					SubscriptionStatus = account.SubscriptionStatus,
					SubscriptionStart = account.SubscriptionStart,
					SubscriptionEnd = account.SubscriptionEnd
				}
			};
		}	
	}
}
