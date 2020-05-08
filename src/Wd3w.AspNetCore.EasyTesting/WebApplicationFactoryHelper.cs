using Microsoft.AspNetCore.Mvc.Testing;

namespace Wd3w.AspNetCore.EasyTesting
{
    public static class WebApplicationFactoryHelper
    {
        public static SystemUnderTest<TStartup> AsSystemUnderTest<TStartup>(
            this WebApplicationFactory<TStartup> factory) where TStartup : class
        {
            return new SystemUnderTest<TStartup>(factory);
        }
    }
}