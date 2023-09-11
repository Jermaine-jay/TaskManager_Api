using TaskManager.Models.Entities;
using TaskManager.Services.Configurations.Jwt;

namespace TaskManager.Services.Interfaces
{
    public interface IJwtAuthenticator
    {
        Task<JwtToken> GenerateJwtToken(ApplicationUser user);
    }
}
