namespace UMServer.Payloads
{
	public class UserMetadata
	{
		public string UserId { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
	}

	public class AccountInfo
	{
		public UserMetadata User { get; set; }
		public PlanMetadata Plan { get; set; }
		public SubscriptionMetadata Subscription { get; set; }
		public bool IsExpired { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeviceActivated { get; set; }
	}

	public class PlanMetadata
	{
		public int PlanId { get; set; }
		public string PlanDescription { get; set; }
	}

	public class SubscriptionMetadata
	{
		public string SubscriptionStatus { get; set; }
		public ulong SubscriptionStart { get; set; }
		public ulong SubscriptionEnd { get; set; }
	}

	public class TrialMetadata : UserMetadata
	{
		public string OS { get; set; }
		public string OSVersion { get; set; }
		public string Country { get; set; }
		public int PlanId { get; set; }
	}

	public class RegistrationMetadata : UserMetadata
	{
		public int PlanId { get; set; }
	}

	public class LicenseMetadata : UserMetadata
	{
		public string LicenseKey { get; set; }
	}

	public class ApiResult
	{
		//public bool Result { get; set; }
		public int StatusCode { get; set; }
		public string Error { get; set; }
		public string Data { get; set; }

		public ApiResult()
		{
		}

		public ApiResult(string error)
		{
			this.StatusCode = -1;
			this.Error = error;
		}

		public ApiResult(int status, string error)
		{
			this.StatusCode = status;
			this.Error = error;
		}
	}
}
