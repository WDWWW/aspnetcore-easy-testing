using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Wd3w.AspNetCore.EasyTesting
{
    public class SystemUnderTest<TStartup> : SystemUnderTest where TStartup : class
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
            var httpClient = WithWebHostBuilder().CreateClient();
            ServiceProvider = _factory.Services;
            return httpClient;
        }

        public override  HttpClient CreateClient(WebApplicationFactoryClientOptions options)
        {
            var httpClient = WithWebHostBuilder().CreateClient(options);
            ServiceProvider = _factory.Services;
            return httpClient;
        }

        public override  HttpClient CreateDefaultClient(params DelegatingHandler[] handlers)
        {
            var defaultClient = WithWebHostBuilder().CreateDefaultClient(handlers);
            ServiceProvider = _factory.Services;
            return defaultClient;
        }

        public override  HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers)
        {
            var defaultClient = WithWebHostBuilder().CreateDefaultClient(baseAddress, handlers);
            ServiceProvider = _factory.Services;
            return defaultClient;
        }

        private WebApplicationFactory<TStartup> WithWebHostBuilder()
        {
            return _factory.WithWebHostBuilder(ConfigureWebHostBuilder);
        }
    }
}