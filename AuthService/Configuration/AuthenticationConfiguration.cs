using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.Configuration
{
    public class AuthenticationConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public int Lifetime { get; set; }

        public SecurityKey GetSecurityKey() =>
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
    }
}
