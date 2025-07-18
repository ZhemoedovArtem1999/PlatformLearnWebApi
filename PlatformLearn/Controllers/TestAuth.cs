using Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PlatformLearnWebApi.Controllers
{
    public class TestAuth : AuthWebApiControllerBase
    {
        [HttpPost("auth111")]
        public async Task<string> Login(CancellationToken cancellationToken = default)
        {
            var s = UserLogin;
            return "1244";
        }
    }
}
