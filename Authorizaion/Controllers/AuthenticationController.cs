using Authentication.Infrastructure.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {

        [HttpPost("Authentcation")]
        public async Task<Results<Ok<AuthentcationResponseDto>, BadRequest<ValidationProblemDetails>>> Login(AuthentcationDto authentcation, CancellationToken cancellationToken)
        {
            try
            {
                var result = await authenticationService.LoginAsync(authentcation, cancellationToken);
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
