using TaskManager.Models.Entities;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Configurations.Jwt
{
    internal class JwtAuthenticator : IJwtAuthenticator
    {
        public Task<JwtToken> GenerateJwtToken(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
