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
            var builder = WithWebHostBuilder();
            var httpClient = builder.CreateClient();
            ServiceProvider = builder.Services;
            return httpClient;
        }

        public override  HttpClient CreateClient(WebApplicationFactoryClientOptions options)
        {
            var builder = WithWebHostBuilder();
            var httpClient = builder.CreateClient(options);
            ServiceProvider = builder.Services;
            return httpClient;
        }

        public override  HttpClient CreateDefaultClient(params DelegatingHandler[] handlers)
        {
            var builder = WithWebHostBuilder();
            var defaultClient = builder.CreateDefaultClient(handlers);
            ServiceProvider = builder.Services;
            return defaultClient;
        }

        public override  HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers)
        {
            var builder = WithWebHostBuilder();
            var defaultClient = builder.CreateDefaultClient(baseAddress, handlers);
            ServiceProvider = builder.Services;
            return defaultClient;
        }

        private WebApplicationFactory<TStartup> WithWebHostBuilder()
        {
            return _factory.WithWebHostBuilder(ConfigureWebHostBuilder);
        }
    }
}