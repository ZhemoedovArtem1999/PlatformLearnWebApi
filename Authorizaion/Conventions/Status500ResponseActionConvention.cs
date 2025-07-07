using Authentication.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Authentication.Conventions
{
    /// <summary>
    /// Добавляет ответ с кодом <c>500</c> если его не было явно добавлено ранее.
    /// </summary>
    internal class Status500ResponseActionConvention : IActionModelConvention
    {
        static readonly ProducesResponseTypeAttribute _filter = new(typeof(ProblemDetails), StatusCodes.Status500InternalServerError);

        public void Apply(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible != true) return;

            if (!action.HasResponseCode(_filter.StatusCode))
            {
                action.Filters.Add(_filter);
            }
        }
    }
}