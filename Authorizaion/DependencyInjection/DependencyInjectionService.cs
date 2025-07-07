using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DependencyInjection
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection DependencyInjectionAuthentication(this IServiceCollection services)
        {


            return services;
        }

    }
}
