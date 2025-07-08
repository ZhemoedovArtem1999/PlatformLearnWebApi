using AuthService.Configuration;
using AuthService.Infrastructure.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Services;
using DataAccessLayer.Dependency;


namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGrpc();
            builder.Services.DependencyInjectionDataAccessLayer(builder.Configuration);
            builder.Services.AddScoped<ITokenManager, JwtTokenManager>();
            builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication"));



            var app = builder.Build();

            app.MapGrpcService<AuthServiceImpl>();

            app.MapGet("/", () => "");

            app.Run();
        }
    }
}