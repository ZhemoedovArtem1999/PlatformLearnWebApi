using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer.Dependency
{
    public static class DependencyDataAccessLayer
    {
        public static IServiceCollection DependencyInjectionDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MainDb");

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            return services;
        }
    }
}
