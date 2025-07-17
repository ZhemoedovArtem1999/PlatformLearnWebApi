using AuthService.Infrastructure.Interfaces;
using AuthService.Models;
using DataAccessLayer.Models;
using Grpc.Core;
using GrpcContracts;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace AuthService.Services
{
    public class AuthServiceImpl(AppDbContext _dbContext, ITokenManager tokenManager) : GrpcContracts.AuthService.AuthServiceBase
    {
        private readonly int Iterations = 99999;
        private readonly int HashSize = 32;

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

            var user = _dbContext.Users.Where(x => x.Email == request.Login).FirstOrDefault();
            if (user == null) throw new Exception("Неверный логин или пароль");

            // TODO:  тут добавить обработку пароля в хэш с солью

            if (user.Password == GetHashPassword(request.Password, user.Salt))
            {
                message = "Аутентификация пройдена";
                return (true, message);
            }

            return (false, message);
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            try
            {
                var user = _dbContext.Users.Where(x => x.Email == request.Email).FirstOrDefault();
                if (user != null) throw new Exception("Пользователь с такой почтой уже зарегистрирован!!!");
                if (request.Password != request.Password2) throw new Exception("Пароли не совпадают!");

                var newUser = new User();
                newUser.Email = request.Email;
                newUser.RoleId = 1; // TODO: Вернуться и исправить магическое число
                newUser.LastName = request.Lastname;
                newUser.FirstName = request.Firstname;
                newUser.MiddleName = request.Middlename;
                newUser.DateBirth = DateOnly.FromDateTime(request.DateBirth.ToDateTime());
                newUser.Gender = request.Gender;
                newUser.Salt = Guid.NewGuid().ToString();
                newUser.Password = GetHashPassword(request.Password, newUser.Salt);

                _dbContext.Add(newUser);
                _dbContext.SaveChanges();

                return Task.FromResult(new RegisterResponse { Success = true, Message = "Регистрация прошла успешно!"});
            }
            catch (Exception ex)
            {
                return Task.FromResult(new RegisterResponse { Message = ex.Message, Success = false });
            }


        }

        private string GetHashPassword(string password, string salt)
        {
            return Convert.ToBase64String(Pbdkf2(password, salt));
        }

        private byte[] Pbdkf2(string password, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                      password: password,
                      salt: saltBytes,
                      iterations: 1,
                      hashAlgorithm: HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

    }
}
