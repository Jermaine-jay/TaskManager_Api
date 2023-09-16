using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Data.Context;
using TaskManager.Models.Entities;

namespace TaskManager.Data.Seeds
{
    public static class DatabaseSeedRoleClaims
    {
        public static async System.Threading.Tasks.Task ClaimSeeder(this IApplicationBuilder app)
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


                /*if (!project)
                {
                    await context.Projects.AddRangeAsync(GetProject(jo));
                    await context.SaveChangesAsync();
                }*/
            }
        }

        private static ICollection<ApplicationRoleClaim> UserClaim(ApplicationRole role)
        {
            return new List<ApplicationRoleClaim>()
            {
                new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "change-password",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "my-account",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "user-tasks",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "user-projects",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "project-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-user-tasks",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-notifications",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "create-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-priority",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-status",
                },

            };
        }


        private static ICollection<ApplicationRoleClaim> AdminClaim(ApplicationRole role)
        {
            return new List<ApplicationRoleClaim>()
            {
                new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "all-users",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "remove-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-a-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "lock-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "all-user-projects",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "all-users-projects",
                },

            };
        }
    }
}
