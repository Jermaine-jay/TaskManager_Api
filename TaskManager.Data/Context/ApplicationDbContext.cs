using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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
        public DbSet<UserTask> UserTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserTask>()
            .HasKey(uta => new { uta.UserId, uta.TaskId });
            

            modelBuilder.Entity<UserTask>()
                .HasOne(uta => uta.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(uta => uta.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
               .HasMany(project => project.Tasks)
               .WithOne(task => task.Project)
               .HasForeignKey(task => task.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserTask>()
                .HasOne(uta => uta.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(uta => uta.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Projects)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()        
                .HasMany(n => n.Notifications)
                .WithOne(u => u.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>()
                .HasMany(task => task.Tasks)
                .WithOne(project => project.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);


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

