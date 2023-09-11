using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Data.Context;
using TaskManager.Models.Entities;
using TaskManager.Services.Configurations.Email;
using TaskManager.Services.Configurations.Jwt;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;

namespace TaskManager.Api.Extensions
{
    
        public static class ServiceExtensions
        {
            public static void RegisterServices(this IServiceCollection services)
            {
                    services.AddScoped<IJwtAuthenticator, JwtAuthenticator>();
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


        public static void ConfigureJWT(this IServiceCollection services, JwtConfig jwtConfig)
        {
            var jwtSettings = jwtConfig;
            var secretKey = jwtSettings.Secret;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })


            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                };
            });

            /*services.AddAuthorization(options =>
            {
                options.AddPolicy("Authorization", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AuthRequirement());
                    policy.Build();
                });
            });*/
        }

        public static void ConfigurationBinder(this IServiceCollection services, IConfiguration configuration)
        {
            Settings setting = configuration.Get<Settings>()!;
            services.AddSingleton(setting);

            JwtConfig jwtConfig = setting.JwtConfig;
            services.AddSingleton(jwtConfig);

            /*RedisConfig redisConfig = setting.redisConfig;
            services.AddSingleton(redisConfig);*/

            ZeroBounceConfig zeroBounceConfig = setting.ZeroBounceConfig;
            services.AddSingleton(zeroBounceConfig);

            EmailSenderOptions emailSenderOptions = setting.EmailSenderOptions;
            services.AddSingleton(emailSenderOptions);

            /*Authentication authentication = setting.Authentication;
            services.AddSingleton(authentication);*/

            services.ConfigureJWT(jwtConfig);
        }
    }
}
