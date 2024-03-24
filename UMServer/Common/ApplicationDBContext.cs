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
            //this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PremiumUser>()
                .HasNoKey();
		}
        public DbSet<Account> Accounts { get; set; }
        public DbSet<PremiumUser> PremiumUsers { get; set; }
        public DbSet<Plan> Plans { get; set; }
    }
}
