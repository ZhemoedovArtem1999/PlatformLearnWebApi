using Core.Controllers;
using GrpcContracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformLearn.Models.Auth;

namespace PlatformLearn.Controllers
{
   
    public class AuthController(AuthService.AuthServiceClient authClient) : WebApiControllerBase
    {
        [HttpPost("auth")]
        public async Task<Results<Ok<AuthentcationResponseDto>, BadRequest<ValidationProblemDetails>>> Login(AuthentcationDto authentication, CancellationToken cancellationToken = default)
        {
            try
            {
                var rpcLoginRequest = new LoginRequest
                {
                    Login = authentication.Login,
                    Password = authentication.Password
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
    }
}
