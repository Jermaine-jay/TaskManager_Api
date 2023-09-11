using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entities;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApplicationUser>()        
                 .HasMany(n => n.Notifications)
                 .WithOne(u => u.ApplicationUser)
                 .HasForeignKey(n => n.ApplicationUserId)
                 .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>()
              .HasMany(t => t.Tasks)
              .WithOne(p => p.Project)
              .HasForeignKey(t => t.ProjectId)
              .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ApplicationUser>()
              .HasMany(u => u.Projects)
              .WithOne(n => n.ApplicationUser)
              .HasForeignKey(n => n.ApplicationUserId)
              .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
              .HasMany(u => u.Projects)
              .WithOne(n => n.ApplicationUser)
              .HasForeignKey(n => n.ApplicationUserId)
              .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.HasMany<ApplicationUserRole>()
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            });


            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany<ApplicationUserRole>()
               .WithOne()
               .HasForeignKey(ur => ur.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

    }

}

