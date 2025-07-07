using Authentication.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Authentication.Conventions
{
    /// <summary>
    /// Добавляет ответ с кодом <c>400</c> если его не было явно добавлено ранее и метод <see cref="ActionModel"/> содержит параметры вызова.
    /// </summary>
    internal class Status400ResponseActionConvention : IActionModelConvention
    {
        static readonly ProducesResponseTypeAttribute _filter = new(typeof(BadRequest<ValidationProblemDetails>), StatusCodes.Status400BadRequest);

        public void Apply(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible != true) return;

            if (action.Parameters.Any(p => p.ParameterType != typeof(CancellationToken)) && !action.HasResponseCode(_filter.StatusCode))
            {
                action.Filters.Add(_filter);
            }
        }
    }
}
