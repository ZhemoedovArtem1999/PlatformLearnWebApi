
using AuthService.Models;

namespace AuthService.Infrastructure.Interfaces
{
    public interface ITokenManager
    {
        string GenerateToken(ApplicationUser appUser);

    }
}
