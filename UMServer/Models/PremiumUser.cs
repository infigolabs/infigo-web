using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace UMServer.Models
{
    public class PremiumUser
    {
        [ForeignKey("User")]
        public string LicenseKey { get; set; }

        public int PlanId { get; set; }
        public virtual Plan Plan { get; set; }
    }
}
