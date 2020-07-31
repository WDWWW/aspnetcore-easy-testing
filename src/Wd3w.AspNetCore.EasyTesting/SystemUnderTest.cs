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
            return WithWebHostBuilder(builder => builder.CreateClient());
        }

        public override  HttpClient CreateClient(WebApplicationFactoryClientOptions options)
        {
            return WithWebHostBuilder(builder => builder.CreateClient(options));
        }

        public override  HttpClient CreateDefaultClient(params DelegatingHandler[] handlers)
        {
            return WithWebHostBuilder(builder => builder.CreateDefaultClient(handlers));
        }

        public override  HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers)
        {
            return WithWebHostBuilder(builder => builder.CreateDefaultClient(baseAddress, handlers));
        }
        
        private WebApplicationFactory<TStartup> WithWebHostBuilder()
        {
            return _factory.WithWebHostBuilder(ConfigureWebHostBuilder);
        }

        private HttpClient WithWebHostBuilder(Func<WebApplicationFactory<TStartup>, HttpClient> createClient)
        {
            var builder = WithWebHostBuilder();
            var httpClient = createClient(builder);
            ServiceProvider = builder.Services;
            ExecuteSetupFixture();
            return httpClient;
        }
    }
}