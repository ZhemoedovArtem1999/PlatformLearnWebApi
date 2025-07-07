using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace Authentication.Filters
{
    /// <summary>
    /// Записывает в лог и оборачивает любую не обработанную ошибку контроллера в ответ с кодом <see cref="StatusCodes.Status499ClientClosedRequest"/> или <see cref="StatusCodes.Status500InternalServerError"/> по ситуации.
    /// </summary>
    /// <param name="logger">Если передать логгер, то ошибка будет записана в лог.</param>
    internal class ActionExceptionFilter(ILogger<ActionExceptionFilter> logger) : IExceptionFilter
    {
        const string _errorMessage = $"{nameof(ActionExceptionFilter)} exception catch";

        public void OnException(ExceptionContext context)
        {
            logger?.LogError(context.Exception, _errorMessage);

            var result = CreateResult(context);

            context.Result = result;

            context.ExceptionHandled = true;
        }

        static IActionResult CreateResult(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                return new JsonResult("Операция отменена")
                {
                    StatusCode = StatusCodes.Status499ClientClosedRequest,
                    ContentType = MediaTypeNames.Application.Json
                };
            }
            var problems = new ProblemDetails()
            {
                Type = context.Exception.HelpLink ?? context.Exception.GetType().ToString(), // содержимое отличается от требований https://datatracker.ietf.org/doc/html/rfc7807, но так сделано для удобства клиента
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected error",
                Instance = context.ActionDescriptor.DisplayName, // содержимое отличается от требований https://datatracker.ietf.org/doc/html/rfc7807, но так сделано для удобства клиента
                Detail = context.Exception.Message,
            };

            var result = new ObjectResult(problems)
            {
                StatusCode = problems.Status,
            };

            result.ContentTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Application.ProblemJson));

            return result;
        }
    }
}