using Authentication.Infrastructure.Interfaces;
using Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Authentication.Infrastructure.Services
{
    public class JwtTokenManager : ITokenManager
    {
        private readonly AuthenticationConfiguration _authenticationConfiguration;

        public JwtTokenManager(IOptions<AuthenticationConfiguration> authenticationConfiguration)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
        }

        public string GenerateToken(ApplicationUser appUser)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _authenticationConfiguration.Issuer,
                Audience = _authenticationConfiguration.Audience,
                Expires = DateTime.Now.AddMinutes(_authenticationConfiguration.Lifetime),
                Subject = appUser.Identity,
                SigningCredentials = new SigningCredentials(_authenticationConfiguration.GetSecurityKey(), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

    }
}
