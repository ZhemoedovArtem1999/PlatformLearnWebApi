using Authentication.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Authentication.Conventions
{
    /// <summary>
    /// Добавляет ответ с кодом <c>499</c> если его не было явно добавлено ранее и метод <see cref="ActionModel"/> содержит <see cref="CancellationToken"/>.
    /// </summary>
    internal class Status499ResponseActionConvention : IActionModelConvention
    {
        static readonly ProducesResponseTypeAttribute _filter = new(typeof(string), StatusCodes.Status499ClientClosedRequest);

        public void Apply(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible != true) return;

            if (action.Parameters.Any(p => p.ParameterType == typeof(CancellationToken)) && !action.HasResponseCode(_filter.StatusCode))
            {
                action.Filters.Add(_filter);
            }
        }
    }
}