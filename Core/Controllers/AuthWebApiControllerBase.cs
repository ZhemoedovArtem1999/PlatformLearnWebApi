using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controllers
{
    /// <summary>
    /// Абстрактный класс для контроллеров, требующих авторизации. 
    /// <remarks>
    /// Добавляет атрибут <see cref="AuthorizeAttribute"/> и наследует остальные от <see cref="WebApiControllerBase"/>.
    /// </remarks>
    /// </summary>
    [Authorize]
    public abstract class AuthWebApiControllerBase : WebApiControllerBase
    {
        /// <summary>
        /// <see cref="ClaimsPrincipal.Identity.Name"/> текущего  пользователя.
        /// </summary>
        protected string UserLogin => User?.Identity?.Name ?? throw new NullReferenceException("Текущий пользователь не определен");
    }
}
