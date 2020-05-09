using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Wd3w.AspNetCore.EasyTesting
{
    public class SystemUnderTest<TStartup> : SystemUnderTestBase where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> _factory;

        public SystemUnderTest()
        {
            _factory = new WebApplicationFactory<TStartup>();
        }

        public SystemUnderTest(WebApplicationFactory<TStartup> factory)
        {
            _factory = factory;
        }

        public override void Dispose()
        {
            _factory?.Dispose();
        }

        public override HttpClient CreateClient()
        {
            return WithWebHostBuilder().CreateClient();
        }

        public override  HttpClient CreateClient(WebApplicationFactoryClientOptions options)
        {
            return WithWebHostBuilder().CreateClient(options);
        }

        public override  HttpClient CreateDefaultClient(params DelegatingHandler[] handlers)
        {
            return WithWebHostBuilder().CreateDefaultClient(handlers);
        }

        public override  HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers)
        {
            return WithWebHostBuilder().CreateDefaultClient(baseAddress, handlers);
        }

        private WebApplicationFactory<TStartup> WithWebHostBuilder()
        {
            return _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(ConfigureTestServices));
        }
    }
}