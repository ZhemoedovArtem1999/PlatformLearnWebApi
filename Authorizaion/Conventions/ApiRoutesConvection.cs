using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace Authentication.Conventions
{
    /// <summary>
    /// Добавляет префикс <see cref="apiName"/> к началу пути и переводит все маршруты и параметры в kebab-case.
    /// </summary>
    /// <param name="apiName">Префикс, который будет добавлен к маршруту.</param>
    internal class ApiRoutesConvection(string? apiName) : IApplicationModelConvention
    {
        class ToLowerKebabParameterTransformer : IOutboundParameterTransformer
        {
            public string? TransformOutbound(object? value) => PascalToKebabLowerCase(value?.ToString());
        }

        static readonly Regex PascalToKebabCaseRegex = new ("(?<!^|/|-|{.*)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex ToLowerExceptParametersRegex = new ("(?<!{.*)[A-Z]*", RegexOptions.Singleline | RegexOptions.Compiled);

        static readonly IOutboundParameterTransformer _transformer = new ToLowerKebabParameterTransformer();

        static string? PascalToKebabLowerCase(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            value = PascalToKebabCaseRegex.Replace(value, string.Intern("-$1"));
            value = ToLowerExceptParametersRegex.Replace(value, m => m.Value.ToLower());

            return value;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var action in controller.Actions.Where(a => a.ApiExplorer.IsVisible == true))
                {
                    foreach (var selector in action.Selectors)
                    {
                        var routeTemplate = selector.AttributeRouteModel?.Template ?? action.ActionName;

                        if (string.IsNullOrEmpty(routeTemplate)) continue;

                        var route = new RouteAttribute(PascalToKebabLowerCase(routeTemplate)!);

                        selector.AttributeRouteModel = new AttributeRouteModel(route);
                    }
                    
                    if (action.RouteParameterTransformer is not null) continue;

                    action.RouteParameterTransformer = _transformer;
                }

                foreach (var selector in controller.Selectors)
                {
                    var template = string.IsNullOrWhiteSpace(apiName) ? selector.AttributeRouteModel?.Template
                        : string.IsNullOrWhiteSpace(selector.AttributeRouteModel?.Template) ? apiName
                            : selector.AttributeRouteModel.Template.StartsWith(apiName) == true ? selector.AttributeRouteModel.Template
                                : AttributeRouteModel.CombineTemplates(apiName, selector.AttributeRouteModel.Template);

                    if (string.IsNullOrWhiteSpace(template)) continue;

                    // Проверка на случай если в пути есть [area], а у контроллера нет такого атрибута (например наследовались от WebApiControllerBase)
                    if (template.Contains("[area]") && !controller.Attributes.Any(a => a is AreaAttribute))
                    {
                        template = template.Replace("[area]", "").Replace("//", "/");

                        if (string.IsNullOrWhiteSpace(template)) continue;
                    }

                    var route = new RouteAttribute(PascalToKebabLowerCase(template)!);

                    selector.AttributeRouteModel = new AttributeRouteModel(route);
                }
            }
        }
    }
}
