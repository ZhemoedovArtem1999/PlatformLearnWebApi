using Authentication.Infrastructure.Interfaces;
using Authentication.Models;
using DataAccessLayer.Models;

namespace Authentication.Infrastructure.Services
{
    public class AuthenticationService(AppDbContext context, ITokenManager tokenManager) : IAuthenticationService
    {

        public async Task<AuthentcationResponseDto> LoginAsync(AuthentcationDto authentcation, CancellationToken cancellationToken = default)
        {

            return await Task.Run(() =>
            {
                var response = new AuthentcationResponseDto();

                var user = context.Users.Where(x => x.Login == authentcation.Login).FirstOrDefault();

                if (user == null) throw new Exception("Неверный логин или пароль");

                // TODO:  тут добавить обработку пароля в хэш с солью

                if (user.Password == authentcation.Password)
                {
                    response.Username = "Все отлично";
                }
                else throw new Exception("Неверный логин или пароль");

                return response;

            }, cancellationToken);
        }
    }
}
