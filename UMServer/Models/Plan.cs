using System.ComponentModel.DataAnnotations.Schema;

namespace UMServer.Models
{
    public class Plan
    {
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int PlanId { get; set; }
        public int PlanLength { get; set; }
        public double PlanPrice { get; set; }
        public string PlanDescription { get; set; }
    }
}
