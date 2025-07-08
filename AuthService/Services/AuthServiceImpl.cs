using AuthService.Infrastructure.Interfaces;
using AuthService.Models;
using DataAccessLayer.Models;
using Grpc.Core;
using GrpcContracts;
using System.Security.Claims;
using System.Threading;

namespace AuthService.Services
{
    public class AuthServiceImpl(AppDbContext _dbContext, ITokenManager tokenManager) : GrpcContracts.AuthService.AuthServiceBase
    {
        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            (bool isValid, string message) = IsValid(request);

            if (isValid)
            {
                var appUser = new ApplicationUser()
                {
                    Identity = new ClaimsIdentity(
                      [new Claim(ClaimsIdentity.DefaultNameClaimType, request.Login)],
                      "Token",
                      ClaimsIdentity.DefaultNameClaimType,
                      ClaimsIdentity.DefaultRoleClaimType
                  )
                };

                var token = tokenManager.GenerateToken(appUser);

                return Task.FromResult(new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Username = appUser.Identity.Name,
                    Message = message,

                });
            }
            else
            {
                return Task.FromResult(new LoginResponse
                {
                    Success = false,
                    Message = message
                });
            }
        }

        private (bool, string) IsValid(LoginRequest request)
        {
            string message = "Неверный логин или пароль";

            var user = _dbContext.Users.Where(x => x.Login == request.Login).FirstOrDefault();
            if (user == null) throw new Exception("Неверный логин или пароль");

            // TODO:  тут добавить обработку пароля в хэш с солью

            if (user.Password == request.Password)
            {
                message = "Аутентификация пройдена";
                return (true, message);
            }

            return (false, message);
        }
    }
}
