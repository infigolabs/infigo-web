using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace UMServer.Models
{
    public class PremiumUser
    {
        [ForeignKey("Account")]
        public string AccountId { get; set; }

		[ForeignKey("Plan")]
		public int PlanId { get; set; }
    }
}
