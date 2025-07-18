using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PlatformLearnWebApi.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Проверяем, есть ли [Authorize] в методе или его родителях
            var hasAuthorize =
                context.MethodInfo.GetCustomAttribute<AuthorizeAttribute>() != null ||
                context.MethodInfo.DeclaringType?.GetCustomAttribute<AuthorizeAttribute>() != null;

            if (hasAuthorize)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }]
                    = Array.Empty<string>()
                }
            };
            }
        }

    }
}
