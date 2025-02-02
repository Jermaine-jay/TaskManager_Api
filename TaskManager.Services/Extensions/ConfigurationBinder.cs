using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services.Configurations.Cache.CacheServices;
using TaskManager.Services.Configurations.Email;
using TaskManager.Services.Configurations.Jwt;
using TaskManager.Services.Infrastructure;

namespace TaskManager.Services.Extensions
{
    public static class ConfigurationBinder
    {
        public static IServiceCollection BindConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            Settings settings = new();
            JwtConfig JwtConfig = new();
            RedisConfig redisConfig = new();
            AppConstants appConstants = new();
            ZeroBounceConfig ZeroBounceConfig = new();
            EmailSenderOptions EmailSenderOptions = new();

            configuration.GetSection("JwtConfig").Bind(JwtConfig);
            configuration.GetSection("RedisConfig").Bind(redisConfig);
            configuration.GetSection("AppConstants").Bind(appConstants);
            configuration.GetSection("ZeroBounceConfig").Bind(ZeroBounceConfig);
            configuration.GetSection("EmailSenderOptions").Bind(EmailSenderOptions);

            services.AddSingleton(settings);
            services.AddSingleton(JwtConfig);
            services.AddSingleton(redisConfig);
            services.AddSingleton(appConstants);
            services.AddSingleton(ZeroBounceConfig);
            services.AddSingleton(EmailSenderOptions);

            return services;
        }
    }
}
