using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Authentication;
using System.Text;
using TaskManager.Data.Context;
using TaskManager.Models.Entities;
using TaskManager.Services.Configurations.Cache.CacheServices;
using TaskManager.Services.Configurations.Cache.Otp;
using TaskManager.Services.Configurations.Email;
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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtAuthenticator, JwtAuthenticator>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IGenerateEmailPage, GenerateEmailPage>();
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
                options.UseSqlServer(configuration, s =>
                {
                    s.MigrationsAssembly("TaskManager.Migrations");
                    s.EnableRetryOnFailure(3);
                });
            });
        }


        public static void ConfigureIdentity(this IServiceCollection services)
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

        public static void AddRedisCache(this IServiceCollection services, RedisConfig redisConfig)
        {

            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.SslProtocols = SslProtocols.Tls12;
            configurationOptions.SyncTimeout = 30000;
            configurationOptions.Ssl = true;
            configurationOptions.Password = redisConfig.Password;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.EndPoints.Add(redisConfig.Host, redisConfig.Port);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationOptions.ToString();
                options.InstanceName = redisConfig.InstanceId;
            });

            services.AddSingleton<IConnectionMultiplexer>((x) =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    Password = configurationOptions.Password,
                    EndPoints = { configurationOptions.EndPoints[0] },
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    ClientName = redisConfig.InstanceId
                });
                return connectionMultiplexer;
            });
            services.AddTransient<ICacheService, CacheService>();
        }


        public static void ConfigurationBinder(this IServiceCollection services, IConfiguration configuration)
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

            /*Authentication authentication = setting.Authentication;
            services.AddSingleton(authentication);*/

            services.ConfigureJWT(jwtConfig);
            services.AddRedisCache(redisConfig);
        }
    }
}
