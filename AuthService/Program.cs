using AuthService.Configuration;
using AuthService.Infrastructure.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Services;
using DataAccessLayer.Dependency;
using Microsoft.AspNetCore.Builder;
using System.IO.Compression;
using System.Runtime;


namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Grpc-Status", "Grpc-Message",
                            "Grpc-Encoding", "Grpc-Accept-Encoding",
                            "Grpc-Status-Details-Bin");
            }));

            builder.Services.AddGrpc(options => {
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
            });
            builder.Services.DependencyInjectionDataAccessLayer(builder.Configuration);
            builder.Services.AddScoped<ITokenManager, JwtTokenManager>();
            builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication"));

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
            builder.Services.AddAuthorization();
            var app = builder.Build();
            app.UseCors();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGrpcService<AuthServiceImpl>().EnableGrpcWeb()
                .RequireCors("AllowAll");
                //.RequireHost("*:5777"); // явное указание хоста
            app.Run();
        }
    }
}