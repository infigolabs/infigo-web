using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UMServer.Models;

namespace UMServer.Common
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
          : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<PremiumAccount> PremiumUsers { get; set; }
        public DbSet<Plan> Plans { get; set; }
    }
}
