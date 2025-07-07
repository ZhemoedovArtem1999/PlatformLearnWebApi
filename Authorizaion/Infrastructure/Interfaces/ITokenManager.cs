using Authentication.Models;

namespace Authentication.Infrastructure.Interfaces
{
    public interface ITokenManager
    {
        string GenerateToken(ApplicationUser appUser);

    }
}
