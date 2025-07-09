using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    /// <summary>
    /// Абстрактный класс для контроллеров. Без авторизации.
    /// <remarks>
    /// Добавляет атрибуты <see cref="ApiControllerAttribute"/> и <see cref="RouteAttribute"/> с маршрутом <c>[area]/[controller]</c>.
    /// </remarks>
    /// </summary>
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public abstract class WebApiControllerBase : ControllerBase
    {

    }
}
