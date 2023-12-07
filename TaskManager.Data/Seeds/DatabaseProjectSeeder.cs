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
                ApplicationUser jo2 = await userManager.FindByEmailAsync("mosalah11@outlook.com");

                context.Database.EnsureCreated();

                var project = context.Projects.Any();


                if (!project)
                {
                    await context.Projects.AddRangeAsync(GetProject(jo));
                    await context.Projects.AddRangeAsync(GetProject(jo2));
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
                    CreatedAt = DateTime.UtcNow,
                    Tasks =  new List<TaskManager.Models.Entities.Task>
                    {
                        new Models.Entities.Task
                        {
                            Id= Guid.NewGuid(),
                            Title = "TestTask",
                            Description = "TestTaskDescription1",
                            Priority = Priority.Low,
                            Status = Status.Pending,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-11-29"), DateTimeKind.Utc)
                        },

                        new Models.Entities.Task
                        {
                            Id= Guid.NewGuid(),
                            Title = "TestTask2",
                            Description = "TestTaskDescription2",
                            Priority = Priority.High,
                            Status = Status.InProgress,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-11-30"), DateTimeKind.Utc)

                        },

                        new Models.Entities.Task
                        {
                            Id = Guid.NewGuid(),
                            Title = "TestTask3",
                            Description = "TestTaskDescription3",
                            Priority = Priority.Medium,
                            Status = Status.InProgress,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-11-29"), DateTimeKind.Utc)

                        },

                        new Models.Entities.Task
                        {
                            Id = Guid.NewGuid(),
                            Title = "TestTask4",
                            Description = "TestTaskDescription4",
                            Priority = Priority.High,
                            Status = Status.Completed,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-12-01"), DateTimeKind.Utc)

                        }
                    },
                },

                new Project()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test2",
                    Description = "Test2",
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Tasks =  new List<TaskManager.Models.Entities.Task>
                    {
                    new Models.Entities.Task
                    {
                        Id= Guid.NewGuid(),
                        Title = "TestTask",
                        Description = "TestTaskDescription1",
                        Priority = Priority.Low,
                        Status = Status.Pending,
                        DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-11-29"), DateTimeKind.Utc)
                    },

                    new Models.Entities.Task
                    {
                        Id= Guid.NewGuid(),
                        Title = "TestTask2",
                        Description = "TestTaskDescription2",
                        Priority = Priority.High,
                        Status = Status.InProgress,
                        DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-11-30"), DateTimeKind.Utc)

                    },

                    new Models.Entities.Task
                    {
                        Id = Guid.NewGuid(),
                        Title = "TestTask3",
                        Description = "TestTaskDescription3",
                        Priority = Priority.Medium,
                        Status = Status.InProgress,
                        DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-11-29"), DateTimeKind.Utc)

                    },

                    new Models.Entities.Task
                    {
                        Id = Guid.NewGuid(),
                        Title = "TestTask4",
                        Description = "TestTaskDescription4",
                        Priority = Priority.High,
                        Status = Status.Completed,
                        DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-12-01"), DateTimeKind.Utc)

                    }
                    },
                },

                new Project()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test3",
                    Description = "Test3",
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Tasks = new List<TaskManager.Models.Entities.Task>
                    {
                        new Models.Entities.Task
                        {
                            Title = "Second TestTask",
                            Description = "Second TestTaskDescription1",
                            Priority = Priority.Low,
                            Status = Status.Pending,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-10-28"), DateTimeKind.Utc)

                        },

                        new Models.Entities.Task
                        {
                            Title = "Second TestTask2",
                            Description = "Second TestTaskDescription2",
                            Priority = Priority.High,
                            Status = Status.InProgress,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-10-29"), DateTimeKind.Utc)
                        },

                        new Models.Entities.Task
                        {
                            Title = "Second TestTask3",
                            Description = "Second TestTaskDescription3",
                            Priority = Priority.Medium,
                            Status = Status.InProgress,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-10-30"), DateTimeKind.Utc)
                        },

                        new Models.Entities.Task
                        {
                            Title = "Second TestTask4",
                            Description = "Second TestTaskDescription4",
                            Priority = Priority.High,
                            Status = Status.Completed,
                            DueDate = DateTime.SpecifyKind(DateTime.Parse("2023-10-31"), DateTimeKind.Utc)
                        }
                    },
                },
            };
        }
    }
}
