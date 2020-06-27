using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Wd3w.AspNetCore.EasyTesting.Internal
{
    internal static class ServiceCollectionHelper
    {
        internal static ServiceDescriptor FindServiceDescriptor<TService>(this IServiceCollection services)
        {
            return services.First(d => d.ServiceType == typeof(TService));
        }
    }
}