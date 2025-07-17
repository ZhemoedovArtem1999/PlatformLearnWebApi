using Core.Controllers;
using Google.Protobuf.WellKnownTypes;
using GrpcContracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformLearnWebApi.Models.Auth;

namespace PlatformLearnWebApi.Controllers
{
   
    public class AuthController(AuthService.AuthServiceClient authClient) : WebApiControllerBase
    {
        [HttpPost("auth")]
        public async Task<Results<Ok<AuthentcationResponseDto>, BadRequest<ValidationProblemDetails>>> Login(AuthentcationDto authenticationDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var rpcLoginRequest = new LoginRequest
                {
                    Login = authenticationDto.Email,
                    Password = authenticationDto.Password
                };


                var result = await authClient.LoginAsync(rpcLoginRequest, cancellationToken: cancellationToken);

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                return TypedResults.Ok(new AuthentcationResponseDto { Token = result.Token, Username = result.Username });
            }
            catch (Exception ex)
            {
                var problems = new ValidationProblemDetails()
                {
                    Title = ex.Message,
                };

                return TypedResults.BadRequest(problems);
            }
        }

        [HttpPost("register")]
        public async Task<Results<Ok<RegisterResponse>, BadRequest<ValidationProblemDetails>>> Register(RegisterRequestDto register, CancellationToken cancellationToken = default) // TODO: тут проба использования в возврате модели из контракта grpc
        {
            try
            {
                var rpcRegisterRequest = new RegisterRequest
                {
                    Email = register.Email,
                    Password = register.Password,
                    Password2 = register.ConfirmPassword,
                    Lastname = register.LastName,
                    Firstname = register.FirstName,
                    Middlename = register.MiddleName,
                    Gender = register.Gender,
                    DateBirth = Timestamp.FromDateTime(register.DateBirth)
                };



                var result = await authClient.RegisterAsync(rpcRegisterRequest, cancellationToken: cancellationToken);

                return TypedResults.Ok(result);
            }
            catch (Exception ex)
            {
                var problems = new ValidationProblemDetails()
                {
                    Title = ex.Message,
                };

                return TypedResults.BadRequest(problems);
            }
        }
    }
}
