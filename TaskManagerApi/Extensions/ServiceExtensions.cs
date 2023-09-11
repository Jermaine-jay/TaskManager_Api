using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Context;
using TaskManager.Models.Entities;

namespace TaskManager.Api.Extensions
{
    
        public static class ServiceExtensions
        {
            public static void RegisterServices(this IServiceCollection services)
            {
                
            }


            public static void ConfigureCors(this IServiceCollection services) =>
                 services.AddCors(options =>
                 {
                     options.AddPolicy("CorsPolicy", builder =>
                     builder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
                 });

            
            public static void RegisterDbContext(this IServiceCollection services, string? configuration)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseSqlServer(configuration, s =>
                    {
                        s.MigrationsAssembly("TaskManager.Migrations");
                        s.EnableRetryOnFailure(3);
                    });
                });
            }


            public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
            {

                services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
                {
                    o.SignIn.RequireConfirmedAccount = false;
                    o.Password.RequireDigit = true;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 10;
                    o.User.RequireUniqueEmail = true;
                })
                   .AddEntityFrameworkStores<ApplicationDbContext>()
                   .AddDefaultTokenProviders();
            }

            /*public static void ConfigurationBinder(this IServiceCollection services, IConfiguration configuration)
            {
                Settings setting = configuration.Get<Settings>()!;
                services.AddSingleton(setting);

                JwtConfig jwtConfig = setting.JwtConfig;
                services.AddSingleton(jwtConfig);

                RedisConfig redisConfig = setting.redisConfig;
                services.AddSingleton(redisConfig);

                ZeroBounceConfig zeroBounceConfig = setting.ZeroBounceConfig;
                services.AddSingleton(zeroBounceConfig);

                EmailSenderOptions emailSenderOptions = setting.EmailSenderOptions;
                services.AddSingleton(emailSenderOptions);

                Authentication authentication = setting.Authentication;
                services.AddSingleton(authentication);
            }*/
        }
}
