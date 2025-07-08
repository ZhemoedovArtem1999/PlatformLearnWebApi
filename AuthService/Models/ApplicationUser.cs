using System.Security.Claims;

namespace AuthService.Models
{
    public class ApplicationUser
    {
        public required ClaimsIdentity Identity { get; set; }

    }
}
