using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMServer.Models
{
    public class UserDetail
    {
        [Key, ForeignKey("User")]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Country { get; set; }

        public virtual User User { get; set; }
    }
}
