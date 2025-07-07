using Authentication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Authentication.Conventions
{
    /// <summary>
    /// Добавляет ответ с кодом <c>401</c> если его не было явно добавлено ранее и метод <see cref="ActionModel"/> требует авторизации.
    /// </summary>
    internal class Status401ResponseActionConvention : IActionModelConvention
    {
        static readonly ProducesResponseTypeAttribute _filter = new(typeof(UnauthorizedHttpResult), StatusCodes.Status401Unauthorized);

        public void Apply(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible != true) return;

            if ((action.Controller.Attributes.Any(a => a is AuthorizeAttribute) && !action.Attributes.Any(a => a is AllowAnonymousAttribute)) || action.Attributes.Any(a => a is AuthorizeAttribute))
            {
                if (!action.HasResponseCode(_filter.StatusCode))
                {
                    action.Filters.Add(_filter);
                }
            }
        }
    }
}
