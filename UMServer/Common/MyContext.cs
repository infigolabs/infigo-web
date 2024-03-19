using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UMServer.Models;

namespace UMServer.Common
{
    public class MyContext: DbContext
    {
        
        public MyContext(DbContextOptions<MyContext> options)
          : base(options)
        {
            //this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PremiumUser>()
                .HasKey(pu => new { pu.LicenseKey, pu.PlanId });

            //modelBuilder.Entity<UserDetail>().HasNoKey();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<PremiumUser> PremiumUsers { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Plan> Plans { get; set; }
    }
}
