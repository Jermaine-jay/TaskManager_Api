
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Authentication;
using System.Text;
using TaskManager.Api.Attribute;
using TaskManager.Data.Context;
using TaskManager.Data.Implementations;
using TaskManager.Data.Interfaces;
using TaskManager.Models.Entities;
using TaskManager.Services.Configurations.Cache.CacheServices;
using TaskManager.Services.Configurations.Cache.Otp;
using TaskManager.Services.Configurations.Cache.Security;
using TaskManager.Services.Configurations.Jwt;
using TaskManager.Services.Implementations;
using TaskManager.Services.Infrastructure;
using TaskManager.Services.Interfaces;


namespace TaskManager.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddTransient<ICacheService, CacheService>();
            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ILockoutAttempt, LockoutAttempt>();
            services.AddScoped<IAuthorizationHandler, AuthHandler>();
            services.AddScoped<IJwtAuthenticator, JwtAuthenticator>();
            services.AddScoped<IRoleClaimService, RoleClaimService>();
            services.AddScoped<IGenerateEmailPage, GenerateEmailPage>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddSingleton<INotificationServiceFactory, NotificationServiceFactory>();
        }

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
           services.Configure<IISOptions>(options =>
           {
           });


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
                options.UseNpgsql(configuration, s =>
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

            services.AddAuthentication()
                 .AddGoogle(options =>
                 {
                     options.ClientId = configuration["Authentication:Google:ClientId"];
                     options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                 });
        }


        public static void ConfigureJWT(this IServiceCollection services, IConfiguration jwtConfig)
        {
            var secretKey = jwtConfig["JwtConfig:Secret"];

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
                    ValidIssuer = jwtConfig["JwtConfig:Issuer"],
                    ValidAudience = jwtConfig["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authorization", policy =>
                {
                    policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
                    policy.Requirements.Add(new AuthRequirement());
                    policy.Build();
                });
            });
        }


        public static void AddRedisCache(this IServiceCollection services, IConfiguration config)
        {
            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.SslProtocols = SslProtocols.Tls12;
            configurationOptions.SyncTimeout = 30000;
            configurationOptions.Ssl = true;
            configurationOptions.Password = config["RedisConfig:Password"];
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.EndPoints.Add(config["RedisConfig:Host"], 10641);


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationOptions.ToString();
                options.InstanceName = config["RedisConfig:InstanceId"];
            });

            services.AddSingleton<IConnectionMultiplexer>((x) =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    Password = configurationOptions.Password,
                    EndPoints = { configurationOptions.EndPoints[0] },
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    ClientName = config["RedisConfig:InstanceId"]
                });
                return connectionMultiplexer;
            });
        }
    }
}
