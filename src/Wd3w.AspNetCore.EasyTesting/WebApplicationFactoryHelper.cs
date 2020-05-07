using Microsoft.AspNetCore.Mvc.Testing;

namespace Wd3w.AspNetCore.EasyTesting
{
    public static class WebApplicationFactoryHelper
    {
        public static EasyIntergrationTester<TStartup> Easy<TStartup>(this WebApplicationFactory<TStartup> factory) where TStartup : class
        {
            return new EasyIntergrationTester<TStartup>(factory);
        }
    }
}