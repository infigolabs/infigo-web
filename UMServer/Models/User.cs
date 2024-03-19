using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System;
using Newtonsoft.Json;

namespace UMServer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public string LicenseKey { get; set; }
        public string Email { get; set; }
        public string ProductVersion { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime SubscriptionStart { get; set; }
        public DateTime SubscriptionEnd { get; set; }
        public bool Active { get; set; }
        public bool IsExpired { get; set; }
        public bool IsDeviceActive { get; set; }

        // Navigation property for the User-Details relationship
        public virtual UserDetail UserDetails { get; set; }
        [ForeignKey("PlanId")]
        public virtual Plan Plan { get; set; }
    }
}
