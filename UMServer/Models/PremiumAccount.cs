using System.ComponentModel.DataAnnotations.Schema;

namespace UMServer.Models
{
	public class PremiumAccount
    {
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string Id { get; set; }

		[ForeignKey("Plan")]
		public int PlanId { get; set; }
    }
}
