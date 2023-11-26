using TaskManager.Services.Configurations.Cache.CacheServices;
using TaskManager.Services.Configurations.Email;
using TaskManager.Services.Configurations.Jwt;

namespace TaskManager.Services.Infrastructure
{
    public class Settings
    {
        public JwtConfig JwtConfig { get; set; } = null!;
        public RedisConfig redisConfig { get; set; } = null!;
        public ZeroBounceConfig ZeroBounceConfig { get; set; } = null!;
        public EmailSenderOptions EmailSenderOptions { get; set; } = null!;
    }
}
