using System.ComponentModel.DataAnnotations.Schema;

namespace UMServer.Models
{
	public class Account
	{
		public string AccountId { get; set; }
		public string LicenseKey { get; set; }
		public string Email { get; set; }
		public string ProductVersion { get; set; }
		public string SubscriptionStatus { get; set; }
		public ulong SubscriptionStart { get; set; }
		public ulong SubscriptionEnd { get; set; }
		public bool Active { get; set; }
		public bool IsExpired { get; set; }
		public bool IsDeviceActive { get; set; }
		public string OS { get; set; }
		public string OSVersion { get; set; }
		public string Country { get; set; }


		[ForeignKey("PlanId")]
		public int PlanId { get; set; }
		public Plan Plan { get; set; }
	}
}
