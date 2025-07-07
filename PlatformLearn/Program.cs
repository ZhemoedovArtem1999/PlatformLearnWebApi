
using Authentication;
using Authentication.DependencyInjection;
using DataAccessLayer.Dependency;
using Microsoft.OpenApi.Models;
using System.Reflection;


namespace PlatformLearn
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            // Add services to the container.
            var assemblyAuthorizationPath = Path.Combine(AppContext.BaseDirectory, "Authentication.dll");
            var assemblyAuthorizaion = Assembly.LoadFrom(assemblyAuthorizationPath);

            builder.Services.AddControllers().AddApplicationPart(assemblyAuthorizaion);
           
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();

            // Добавление вспомогательных служб
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "PlatformLearn.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(60);
            });

            builder.Services.DependencyInjectionDataAccessLayer(builder.Configuration);
            builder.Services.DependencyInjectionAuthentication();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                // Для библиотек
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                foreach (var xmlFile in xmlFiles)
                {
                    c.IncludeXmlComments(xmlFile);
                }

                c.EnableAnnotations();
                c.IgnoreObsoleteProperties();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseWebApiSwagger("PlatformLearn", "v1");



            app.UseRouting();
            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();


            app.MapControllers();


            app.UseHttpsRedirection();

            await app.RunAsync();
        }
    }
}
