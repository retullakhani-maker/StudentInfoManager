using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.Model;


namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Students> Students { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-One: ApplicationUser ↔ StudentProfile
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.StudentProfile)
                .WithOne(p => p.User)
                .HasForeignKey<Students>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: State → Cities
            modelBuilder.Entity<States>()
                .HasMany(s => s.Cities)
                .WithOne(c => c.State)
                .HasForeignKey(c => c.StateId)
                .OnDelete(DeleteBehavior.Restrict);  // 🚫 prevent cascade loop

            // One-to-Many: State → Students
            modelBuilder.Entity<States>()
                .HasMany<Students>()
                .WithOne(s => s.State)
                .HasForeignKey(s => s.StateId)
                .OnDelete(DeleteBehavior.Restrict);  // 🚫 prevent multiple cascade

            // One-to-Many: City → Students
            modelBuilder.Entity<Cities>()
                .HasMany<Students>()
                .WithOne(s => s.City)
                .HasForeignKey(s => s.CityId)
                .OnDelete(DeleteBehavior.Restrict);  // 🚫 prevent multiple cascade
        }
    }
}
