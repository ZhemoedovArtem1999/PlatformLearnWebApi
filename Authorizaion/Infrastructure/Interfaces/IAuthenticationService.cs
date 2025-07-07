using Authentication.Models;

namespace Authentication.Infrastructure.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthentcationResponseDto> LoginAsync(AuthentcationDto authentcation, CancellationToken cancellationToken = default);
    }
}
