using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet("proba/{aaa}")]
        public ActionResult<string> Proba(long aaa)
        {
            
            return $"aaaa{aaa}";
        }
    }
}
