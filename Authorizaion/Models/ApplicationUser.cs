using System.Security.Claims;

namespace Authentication.Models
{
    public class ApplicationUser
    {
        public required ClaimsIdentity Identity { get; set; }

    }
}
