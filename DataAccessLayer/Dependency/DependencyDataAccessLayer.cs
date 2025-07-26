using Core.RepositoryBase;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using DataAccessLayer.Repository.FilterModel;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataAccessLayer.Dependency
{
    public static class DependencyDataAccessLayer
    {
        public static IServiceCollection DependencyInjectionDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MainDb");

            //services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            var dataAccessTypeStr = configuration["DataAccessType"];
            Enum.TryParse<DataAccessType>(dataAccessTypeStr, out var dataAccessType);

            switch (dataAccessType)
            {
                case DataAccessType.EntityFramework:
                    services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));


                    services.AddScoped<IUnitOfWork<User, UserFilter>>(provider =>
                        new EfUnitOfWork<User, UserFilter>(provider.GetRequiredService<AppDbContext>()));
                    services.AddScoped<IUnitOfWork<Role, FilterBase>>(provider =>
          new EfUnitOfWork<Role, FilterBase>(provider.GetRequiredService<AppDbContext>()));
                    break;

                case DataAccessType.Dapper:
                    services.AddScoped<IUnitOfWork<object, UserFilter>>(_ =>
                        new DapperUnitOfWork<object, UserFilter>(connectionString));
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported DataAccessType: {dataAccessTypeStr}");
            }



            return services;
        }
    }
}
