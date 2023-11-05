using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TaskManager.Data.Context;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using Task = System.Threading.Tasks.Task;


namespace TaskManager.Data.Seeds
{
    public static class DatabaseProjectSeeder
    {
        public static async Task ProjectSeeder(this IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ApplicationDbContext>();


            using (var scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser jo = await userManager.FindByEmailAsync("Jermaine.jay00@gmail.com");

                context.Database.EnsureCreated();

                var project = context.Projects.Any();
                var task = context.Tasks.Any();

                
                if (!project)
                {
                    await context.Projects.AddRangeAsync(GetProject(jo));
                    await context.SaveChangesAsync();
                }
            }

            using(var  scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser jo = await userManager.FindByEmailAsync("Jermaine.jay00@gmail.com");

                context.Database.EnsureCreated();

                var project = context.Projects.Any();
                var task = context.Tasks.Any();

                if (!task)
                {

                    await context.Tasks.AddRangeAsync(GetTasks1(jo.Projects.FirstOrDefault()));
                    await context.Tasks.AddRangeAsync(GetTasks2(jo.Projects.LastOrDefault()));
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task TaskSeeder(this IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ApplicationDbContext>();


            using (var scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser jo = await userManager.FindByEmailAsync("Jermaine.jay00@gmail.com");

                context.Database.EnsureCreated();
                var task = context.Tasks.Any();

                if (!task)
                {
                    await context.Tasks.AddRangeAsync(GetTasks1(jo.Projects.FirstOrDefault()));
                    await context.Tasks.AddRangeAsync(GetTasks2(jo.Projects.LastOrDefault()));
                    await context.SaveChangesAsync();
                }
            }
        }

        private static ICollection<Project> GetProject(ApplicationUser user)
        {
            return new List<Project>()
            {
                new Project()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    Description = "Test",
                    UserId = user.Id,
                },

                new Project()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test2",
                    Description = "Test2",
                    UserId = user.Id,
                },
                new Project()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test3",
                    Description = "Test3",
                    UserId = user.Id,
                },

            };
        }

        private static ICollection<TaskManager.Models.Entities.Task> GetTasks1(Project project)
        {
            return new List<TaskManager.Models.Entities.Task>
            {
                new Models.Entities.Task
                {
                    Id= Guid.NewGuid(), 
                    Title = "TestTask",
                    Description = "TestTaskDescription1",
                    Priority = Priority.Low,
                    Status = Status.Pending,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-29"),
                },

                new Models.Entities.Task
                {
                    Id= Guid.NewGuid(),
                    Title = "TestTask2",
                    Description = "TestTaskDescription2",
                    Priority = Priority.High,
                    Status = Status.InProgress,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-30"),

                },

                new Models.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    Title = "TestTask3",
                    Description = "TestTaskDescription3",
                    Priority = Priority.Medium,
                    Status = Status.InProgress,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-31"),

                },

                new Models.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    Title = "TestTask4",
                    Description = "TestTaskDescription4",
                    Priority = Priority.High,
                    Status = Status.Completed,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-11-01"),

                }
            };

        }

        private static ICollection<TaskManager.Models.Entities.Task> GetTasks2(Project project)
        {
            return new List<TaskManager.Models.Entities.Task>
            {
                new Models.Entities.Task
                {
                    Title = "Second TestTask",
                    Description = "Second TestTaskDescription1",
                    Priority = Priority.Low,
                    Status = Status.Pending,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-28"),

                },

                new Models.Entities.Task
                {
                    Title = "Second TestTask2",
                    Description = "Second TestTaskDescription2",
                    Priority = Priority.High,
                    Status = Status.InProgress,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-29"),

                },

                new Models.Entities.Task
                {
                    Title = "Second TestTask3",
                    Description = "Second TestTaskDescription3",
                    Priority = Priority.Medium,
                    Status = Status.InProgress,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-30"),

                },

                new Models.Entities.Task
                {
                    Title = "Second TestTask4",
                    Description = "Second TestTaskDescription4",
                    Priority = Priority.High,
                    Status = Status.Completed,
                    ProjectId = project.Id,
                    DueDate = DateTime.Parse("2023-10-31"),

                }
            };

        }
    }
}
