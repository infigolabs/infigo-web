namespace UMServer.Models
{
    public class Plan
    {
        public int PlanId { get; set; }
        public int PlanLength { get; set; }
        public decimal PlanPrice { get; set; }
        public string PlanDescription { get; set; }

        // Navigation properties for the User and PremiumUser tables
        public virtual User User { get; set; }
        public virtual PremiumUser PremiumUser { get; set; }
    }
}
