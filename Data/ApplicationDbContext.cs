using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TasksManager.Models;

namespace TasksManager.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<ApplicationUser> UsersDB { get; set; }
        public DbSet<TaskModel> TasksDB { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                    .HasMany(u => u.AssignedTasks)
                    .WithMany(t => t.AssignedUsers);

            builder.Entity<TaskModel>()
                .HasOne(t => t.Owner)
                .WithMany(u => u.OwnedTasks)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
