using Authentication.Conventions;
using Authentication.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Authentication
{
    public static class WebApiConfigurator
    {
        /// <param name="includeXmlComments">Добавить xml-комментарии сборки (<see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/"/>) из файлов xml, найденных в <see cref="AppContext.BaseDirectory"/>.</param>
        /// <inheritdoc cref="AddWebApi(IServiceCollection, string, string, string?, IEnumerable{string})" />
        public static IServiceCollection AddWebApi(this IServiceCollection services, string apiTitle, string apiVersion, string? apiPrefix = null, bool? includeXmlComments = true)
            => includeXmlComments == true ? AddWebApi(services, apiTitle, apiVersion, apiPrefix, Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
            : AddWebApi(services, apiTitle, apiVersion, apiPrefix, []);

        /// <param name="xmlCommentsFilePath">Путь к файлу с xml-комментариями сборки (<see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/"/>) для которой формируется веб апи. Комментарии будут добавлены в спецификацию и Swagger.</param>
        /// <inheritdoc cref="AddWebApi(IServiceCollection, string, string, string?, IEnumerable{string})" />
        public static IServiceCollection AddWebApi(this IServiceCollection services, string apiTitle, string apiVersion, string? apiPrefix, string xmlCommentsFilePath)
            => AddWebApi(services, apiTitle, apiVersion, apiPrefix, [xmlCommentsFilePath]);

        public static IServiceCollection AddWebApi(this IServiceCollection services, string apiTitle, string apiVersion, string? apiPrefix, IEnumerable<string> xmlCommentsFilesPath)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });
                options.AddEnumsWithValuesFixFilters();
                options.EnableAnnotations();

                foreach (var path in xmlCommentsFilesPath)
                {
                    if (AssemblyXmlHelper.IsAssemblyDocXml(path))
                    {
                        options.IncludeXmlComments(path, true);
                    }
                }

                options.DocInclusionPredicate((name, api) => true);

                // Группировка api по тэгу [Area]
                options.TagActionsBy(api =>
                {
                    if (api.ActionDescriptor is ControllerActionDescriptor descriptor)
                    {
                        var areaNames = descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AreaAttribute), true).OfType<AreaAttribute>();

                        if (areaNames.Any())
                        {
                            return areaNames.Select(a => a.RouteValue).ToArray();
                        }
                        else
                        {
                            return [descriptor.ControllerName];
                        }
                    }

                    return [api.GroupName];
                });

                // Сортировка api по тэгу [Area]
                options.OrderActionsBy(api =>
                {
                    if (api.ActionDescriptor is ControllerActionDescriptor descriptor)
                    {
                        var areaNames = descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AreaAttribute), true).OfType<AreaAttribute>();

                        if (areaNames.Any())
                        {
                            return string.Join(" ", areaNames.Select(a => a.RouteValue).Order());
                        }
                        else
                        {
                            return descriptor.ControllerName;
                        }
                    }

                    return api.RelativePath;
                });
            });

            services.AddMvcCore(setup =>
            {
                // Должно быть первым, т.к. видимость используется в остальных конвенциях
                setup.Conventions.Add(new ActionVisibleConvention());

                // Форматируем пути всех маршрутов в kebab-case и добавляем в начале {apiPrefix}/
                if (!string.IsNullOrWhiteSpace(apiPrefix))
                {
                    setup.Conventions.Add(new ApiRoutesConvection(apiPrefix));
                }

                // Добавляем описание возможных ответов api
                setup.Conventions.Add(new Status200ResponseActionConvention());
                setup.Conventions.Add(new Status400ResponseActionConvention());
                setup.Conventions.Add(new Status401ResponseActionConvention());
                setup.Conventions.Add(new Status499ResponseActionConvention());
                setup.Conventions.Add(new Status500ResponseActionConvention());

                //setup.Filters.Add(typeof(ActionExceptionFilter));
            }).AddApiExplorer();

            return services;
        }

        /// <summary>
        /// Добавление Swagger UI для веб апи и спецификации по адресу /swagger/{apiVersion}/swagger.json
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> для конфигурирования.</param>
        /// <param name="apiTitle">Название апи. Будет использоваться в <see cref="OpenApiInfo.Title"/>.</param>
        /// <param name="apiVersion">Версия апи. Будет использоваться в <see cref="OpenApiInfo.Version"/>.</param>
        /// <returns>Возвращает <see cref="IApplicationBuilder"/> для дальнейшего конфигурирования.</returns>
        public static IApplicationBuilder UseWebApiSwagger(this IApplicationBuilder app, string apiTitle, string apiVersion)
        {
            return app
                .UseSwagger()
                .UseSwaggerUI(options => {
                    options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"{apiTitle} {apiVersion}");
                    options.EnableFilter();
                    options.DisplayRequestDuration();
                    options.ShowCommonExtensions();
                });
        }
    }
}
