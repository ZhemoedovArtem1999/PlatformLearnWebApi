
using DataAccessLayer.Dependency;
using GrpcContracts;
using Microsoft.OpenApi.Models;
using PlatformLearnWebApi.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PlatformLearnWebApi
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
                           .AllowAnyHeader()
                           .WithExposedHeaders("Grpc-Status", "Grpc-Message"); ;
                });
            });

            builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                                    ValidAudience = builder.Configuration["Authentication:Audience"],
                                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecurityKey"]))
                                };
                            });



            //var assemblyAuthorizationPath = Path.Combine(AppContext.BaseDirectory, "Authentication.dll");
            //var assemblyAuthorizaion = Assembly.LoadFrom(assemblyAuthorizationPath);

            //builder.Services.AddControllers().AddApplicationPart(assemblyAuthorizaion);


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformLearnWebApi", Version = "v1" });

                // Включите XML комментарии (если нужно)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Используйте имена методов как operationId
                c.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                        ? methodInfo.Name
                        : null;
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>();  // ← Добавьте эту строку

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                });

                // Глобальное требование авторизации
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }


    });




            });
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

            builder.Services.AddGrpcClient<GrpcContracts.AuthService.AuthServiceClient>(options =>
            {
                options.Address = new Uri("http://localhost:5777"); // TODO: заменить из конфигурации или вообще вырезать
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


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
