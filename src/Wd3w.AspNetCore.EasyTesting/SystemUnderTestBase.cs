using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Wd3w.AspNetCore.EasyTesting
{
    public abstract class SystemUnderTestBase : IDisposable
    {
        private IServiceCollection _serviceCollection;

        private IServiceProvider _serviceProvider;

        public IServiceProvider ServiceProvider => _serviceProvider;

        internal IServiceCollection InternalServiceCollection { get; } = new ServiceCollection();

        internal IServiceProvider InternalServiceProvider => InternalServiceCollection.BuildServiceProvider();

        protected delegate void ConfigureTestServiceHandler(IServiceCollection services);

        protected delegate Task SetupFixtureHandler(IServiceProvider provider);

        protected delegate void ConfigureWebHostBuilderHandler(IWebHostBuilder builder);

        protected event SetupFixtureHandler OnSetupFixtures;

        protected event ConfigureTestServiceHandler OnConfigureTestServices;

        protected event ConfigureWebHostBuilderHandler OnConfigureWebHostBuilder;

        protected void CheckClientIsNotCreated(string methodName)
        {
            if (_serviceProvider != default)
                throw new InvalidOperationException(
                    $"{methodName}는 CreateClient/CreateHestify CreateClient/CreateHestify 호출 이전에만 사용할 수 있습니다.");
        }

        protected void CheckClientIsCreated(string methodName)
        {
            if (_serviceProvider == default)
                throw new InvalidOperationException($"{methodName}는 CreateClient/CreateHestify 생성 이후에만 사용할 수 있습니다.");
        }

        public SystemUnderTestBase ReplaceService<TService, TImplementation>(ServiceLifetime? lifetime = default)
        {
            CheckClientIsNotCreated(nameof(ReplaceService));
            OnConfigureTestServices += services =>
            {
                var descriptor = FindOriginServiceDescriptor<TService>(services);
                services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation),
                    lifetime ?? descriptor.Lifetime));
            };
            return this;
        }

        public SystemUnderTestBase ReplaceService<TService>(TService obj)
        {
            CheckClientIsNotCreated(nameof(ReplaceService));
            OnConfigureTestServices += services =>
                services.Replace(new ServiceDescriptor(typeof(TService), _ => obj, ServiceLifetime.Singleton));
            return this;
        }

        public SystemUnderTestBase MockService<TService>(out Mock<TService> mock) where TService : class
        {
            CheckClientIsNotCreated(nameof(MockService));

            var service = InternalServiceProvider.GetService<Mock<TService>>();
            if (service == default)
            {
                mock =  new Mock<TService>();
                InternalServiceCollection.AddSingleton(mock);
                ReplaceService(mock.Object);
            }
            else
            {
                mock = service;
            }

            return this;
        }

        public SystemUnderTestBase MockService<TService>(Action<Mock<TService>> mockAction) where TService : class
        {
            MockService<TService>(out var mock);
            mockAction(mock);
            return this;
        }

        public SystemUnderTestBase SetupFixture<TService>(Func<TService, Task> action)
        {
            CheckClientIsNotCreated(nameof(SetupFixture));
            OnSetupFixtures += provider => action.Invoke(provider.GetService<TService>());
            return this;
        }



        private static ServiceDescriptor FindOriginServiceDescriptor<TService>(IServiceCollection services)
        {
            return services.First(d => d.ServiceType == typeof(TService));
        }

               public async Task UsingServiceAsync(Func<IServiceProvider, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using (_serviceProvider.CreateScope())
            {
                await action.Invoke(_serviceProvider);
            }
        }

        public async Task UsingServiceAsync<TService>(Func<TService, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using (_serviceProvider.CreateScope())
            {
                await action.Invoke(_serviceProvider.GetService<TService>());
            }
        }

        public async Task UsingServiceAsync<TService1, TService2>(Func<TService1, TService2, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using (_serviceProvider.CreateScope())
            {
                await action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>());
            }
        }

        public async Task UsingServiceAsync<TService1, TService2, TService3>(
            Func<TService1, TService2, TService3, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using (_serviceProvider.CreateScope())
            {
                await action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>(),
                    _serviceProvider.GetService<TService3>());
            }
        }

        public void UsingService(Action<IServiceProvider> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider);
            }
        }

        public void UsingService<TService>(Action<TService> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider.GetService<TService>());
            }
        }

        public void UsingService<TService1, TService2>(Action<TService1, TService2> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>());
            }
        }

        public void UsingService<TService1, TService2, TService3>(Action<TService1, TService2, TService3> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>(),
                    _serviceProvider.GetService<TService3>());
            }
        }

        public bool VerifyRegisteredLifeTimeOfService<TService>(ServiceLifetime lifetime)
        {
            if (_serviceCollection == default)
                throw new InvalidOperationException("Should create client before verify service's registered lifetime.");
            var descriptor = _serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(TService))
                ?? throw new InvalidOperationException("The provided service type is not registered from SUT service collection.");

            return descriptor.Lifetime == lifetime;
        }

        public SystemUnderTestBase SetupWebHostBuilder(Action<IWebHostBuilder> configureAction)
        {
            OnConfigureWebHostBuilder += configureAction.Invoke;
            return this;
        }

        public SystemUnderTestBase UseSetting(string key, string value)
        {
            OnConfigureWebHostBuilder += builder => builder.UseSetting(key, value);
            return this;
        }

        public SystemUnderTestBase ConfigureAppConfiguration(Action<IConfigurationBuilder> configureAction)
        {
            OnConfigureWebHostBuilder += builder => builder.ConfigureAppConfiguration(configureAction);
            return this;
        }

        public SystemUnderTestBase ConfigureServices(Action<IServiceCollection> configureServices)
        {
            OnConfigureWebHostBuilder += builder => builder.ConfigureServices(configureServices);
            return this;
        }

        internal void ConfigureWebHostBuilder(IWebHostBuilder builder)
        {
            OnConfigureWebHostBuilder?.Invoke(builder);
            builder.ConfigureTestServices(services =>
            {
                OnConfigureTestServices?.Invoke(services);
                _serviceCollection = services;
                var provider = services.BuildServiceProvider();
                _serviceProvider = provider;

                using (provider.CreateScope())
                {
                    OnSetupFixtures?.Invoke(provider).Wait();
                }
            });
        }

        public abstract HttpClient CreateClient();

        public abstract HttpClient CreateClient(WebApplicationFactoryClientOptions options);

        public abstract HttpClient CreateDefaultClient(params DelegatingHandler[] handlers);

        public abstract HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers);

        public abstract void Dispose();
    }
}