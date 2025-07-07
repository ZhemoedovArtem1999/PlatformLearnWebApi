using Authentication.Infrastructure.Interfaces;
using Authentication.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.DependencyInjection
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection DependencyInjectionAuthentication(this IServiceCollection services)
        {
            services.AddScoped<ITokenManager, JwtTokenManager>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();


            services.AddWebApi("authentification", "1.0.0", "apiAuthentification");


            return services;
        }

    }
}
