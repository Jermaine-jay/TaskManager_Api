using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Data.Seeds
{
    public static class DatabaseUserSeeder
    {
        public static async Task SeededUserAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                if (!await userManager.Users.AnyAsync())
                {
                    foreach (ApplicationUser user in GetUsers())
                    {
                        await userManager.CreateAsync(user, user.PasswordHash);
                    }
                }

                ApplicationUser user1 = await userManager.FindByEmailAsync("jermaine.jay00@gmail.com");
                ApplicationUser user2 = await userManager.FindByEmailAsync("jsonosita@outlook.com");
                ApplicationUser user3 = await userManager.FindByEmailAsync("jsonosii097@gmail.com");
                ApplicationUser user4 = await userManager.FindByEmailAsync("IbouKonate@gmail.com");

                var User = UserType.User.GetStringValue();
                var Admin = UserType.Admin.GetStringValue();
                var SuperAdmin = UserType.SuperAdmin.GetStringValue();

                if (user1 != null)
                {
                    await userManager.AddToRoleAsync(user1, User);
                }

                if (user2 != null)
                {
                    await userManager.AddToRolesAsync(user2, new[] { Admin });
                }

                if (user3 != null)
                {
                    await userManager.AddToRolesAsync(user3, new[] { SuperAdmin });
                    await userManager.AddToRolesAsync(user3, new[] { Admin });
                }

                if (user4 != null)
                {
                    await userManager.AddToRolesAsync(user4, new[] { User });
                }
            }
        }

        private static ICollection<ApplicationUser> GetUsers()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    UserName = "jermaine.jay00@gmail.com",
                    FirstName = "jermaine",
                    LastName = "jay",
                    Email = "jermaine.jay00@gmail.com",
                    PhoneNumber = "1234567890",
                    PasswordHash = "12345qwert",
                    Active = true,
                    UserType = UserType.User,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },

                 new ApplicationUser
                {
                    FirstName = "Mo",
                    LastName = "Salah",
                    Email = "jsonosita@outlook.com",
                    UserName = "jsonosita@outlook.com",
                    PhoneNumber = "1334447880",
                    PasswordHash = "12345qwert",
                    Active = true,
                    UserType = UserType.SuperAdmin,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },

                new ApplicationUser
                {

                    FirstName = "Roberto",
                    LastName = "Firmino",
                    Email = "jsonosii097@gmail.com",
                    UserName = "jsonosii097@gmail.com",
                    PhoneNumber = "1234447890",
                    PasswordHash = "12345qwert",
                    Active = true,
                    UserType = UserType.Admin,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },

                new ApplicationUser
                {
                    FirstName = "Ibou",
                    LastName = "Konate",
                    Email = "IbouKonate@gmail.com",
                    UserName = "IbouKonate@gmail.com",
                    PhoneNumber = "1223344789",
                    PasswordHash = "12345qwert",
                    Active = true,
                    UserType = UserType.User,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }
            };
        }
    }
}
