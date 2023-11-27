using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Models.Entities;
using TaskManager.Services.Interfaces;
using TaskManager.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Models.Dtos.Response;
using Microsoft.Extensions.Configuration;

namespace TaskManager.Services.Configurations.Jwt
{
    public class JwtAuthenticator : IJwtAuthenticator
    {
        private readonly IConfiguration _config;

        public JwtAuthenticator(IConfiguration config)
        {
            _config = config;
        }

        public async Task<JwtToken> GenerateJwtToken(ApplicationUser user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();
            var key = Encoding.ASCII.GetBytes(_config["JwtConfig:Secret"]);

            string userRole = user.UserType.GetStringValue();
            IdentityOptions _options = new();

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, userRole),             
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(300),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JwtConfig:Issuer"],
                Audience = _config["JwtConfig:Audience"]
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new JwtToken
            {
                Token = jwtToken,
                Issued = DateTime.Now,
                Expires = tokenDescriptor.Expires.Value
            };
        }
    }
}
