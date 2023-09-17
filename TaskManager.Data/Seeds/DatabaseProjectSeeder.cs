using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
                    Name = "Test2",
                    Description = "Test2",
                    UserId = user.Id,
                },
                new Project()
                {
                    Name = "Test3",
                    Description = "Test3",
                    UserId = user.Id,
                },

            };
        }

        private static ICollection<TaskManager.Models.Entities.Task> GetTasks(Project project)
        {
            return new List<TaskManager.Models.Entities.Task>
            {
                new Models.Entities.Task
                {
                    Title = "TestTask",
                    Description = "TestTaskDescription",
                    Priority = Priority.Low,
                    Status = Status.Pending,
                    ProjectId = project.Id,
                },

                new Models.Entities.Task
                {
                    Title = "TestTask2",
                    Description = "TestTaskDescription2",
                    Priority = Priority.High,
                    Status = Status.InProgress,
                    ProjectId = project.Id,
                },

                new Models.Entities.Task
                {
                    Title = "TestTask3",
                    Description = "TestTaskDescription3",
                    Priority = Priority.Medium,
                    Status = Status.InProgress,
                    ProjectId = project.Id,
                },

                new Models.Entities.Task
                {
                    Title = "TestTask4",
                    Description = "TestTaskDescription4",
                    Priority = Priority.High,
                    Status = Status.Completed,
                    ProjectId = project.Id,
                }
            };

        }
    }
}
