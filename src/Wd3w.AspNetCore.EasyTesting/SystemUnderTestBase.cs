using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.NSubstitute")]
[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.Moq")]
[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.FakeItEasy")]
[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.Test")]
namespace Wd3w.AspNetCore.EasyTesting
{
    public abstract class SystemUnderTest : IDisposable
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

        internal void CheckClientIsNotCreated(string methodName)
        {
            if (_serviceProvider != default)
                throw new InvalidOperationException(
                    $"{methodName}는 CreateClient/CreateHestify CreateClient/CreateHestify 호출 이전에만 사용할 수 있습니다.");
        }

        internal void CheckClientIsCreated(string methodName)
        {
            if (_serviceProvider == default)
                throw new InvalidOperationException($"{methodName}는 CreateClient/CreateHestify 생성 이후에만 사용할 수 있습니다.");
        }

        /// <summary>
        ///     Replace pre-registered service with TImplementation type with lifetime parameter.
        /// </summary>
        /// <param name="lifetime">If lifetime is null, pre-register service lifetime will be used.</param>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public SystemUnderTest ReplaceService<TService, TImplementation>(ServiceLifetime? lifetime = default)
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

        /// <summary>
        ///     Replace pre-registered service with parameter object, the obj will register as singleton.
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public SystemUnderTest ReplaceService<TService>(TService obj)
        {
            CheckClientIsNotCreated(nameof(ReplaceService));
            OnConfigureTestServices += services =>
                services.Replace(new ServiceDescriptor(typeof(TService), _ => obj, ServiceLifetime.Singleton));
            return this;
        }

        internal TService GetOrAddInternalService<TService>(Func<TService> factory) where TService : class
        {
            InternalServiceCollection.TryAddSingleton(factory());
            return InternalServiceProvider.GetRequiredService<TService>();
        }

        /// <summary>
        ///     Add fixture setting hook, this hook will be called once after register every system or test services.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public SystemUnderTest SetupFixture<TService>(Func<TService, Task> action)
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

        /// <summary>
        ///     Use TService object from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public async Task UsingServiceAsync<TService>(Func<TService, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using (_serviceProvider.CreateScope())
            {
                await action.Invoke(_serviceProvider.GetService<TService>());
            }
        }

        /// <summary>
        ///     Use two service objects from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TService2"></typeparam>
        /// <returns></returns>
        public async Task UsingServiceAsync<TService1, TService2>(Func<TService1, TService2, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using (_serviceProvider.CreateScope())
            {
                await action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>());
            }
        }

        /// <summary>
        ///     Use three service objects from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TService2"></typeparam>
        /// <typeparam name="TService3"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        ///     Use service provider.
        /// </summary>
        /// <param name="action"></param>
        public void UsingService(Action<IServiceProvider> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider);
            }
        }

        /// <summary>
        ///     Use TService object from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService"></typeparam>
        public void UsingService<TService>(Action<TService> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider.GetService<TService>());
            }
        }

        /// <summary>
        ///     Use two service objects from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TService2"></typeparam>
        public void UsingService<TService1, TService2>(Action<TService1, TService2> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>());
            }
        }

        /// <summary>
        ///     Use three service objects from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService1"></typeparam>
        /// <typeparam name="TService2"></typeparam>
        /// <typeparam name="TService3"></typeparam>
        public void UsingService<TService1, TService2, TService3>(Action<TService1, TService2, TService3> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using (_serviceProvider.CreateScope())
            {
                action.Invoke(_serviceProvider.GetService<TService1>(), _serviceProvider.GetService<TService2>(),
                    _serviceProvider.GetService<TService3>());
            }
        }

        /// <summary>
        ///     Verify lifetime of registered services.
        /// </summary>
        /// <param name="lifetime"></param>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool VerifyRegisteredLifeTimeOfService<TService>(ServiceLifetime lifetime)
        {
            if (_serviceCollection == default)
                throw new InvalidOperationException("Should create client before verify service's registered lifetime.");
            var descriptor = _serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(TService))
                ?? throw new InvalidOperationException("The provided service type is not registered from SUT service collection.");

            return descriptor.Lifetime == lifetime;
        }

        /// <summary>
        ///     Setup configure web host builder.
        /// </summary>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public SystemUnderTest SetupWebHostBuilder(Action<IWebHostBuilder> configureAction)
        {
            OnConfigureWebHostBuilder += configureAction.Invoke;
            return this;
        }

        /// <summary>
        ///     Add setting for sut
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SystemUnderTest UseSetting(string key, string value)
        {
            OnConfigureWebHostBuilder += builder => builder.UseSetting(key, value);
            return this;
        }

        /// <summary>
        ///     Hook for configuring application 
        /// </summary>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public SystemUnderTest ConfigureAppConfiguration(Action<IConfigurationBuilder> configureAction)
        {
            OnConfigureWebHostBuilder += builder => builder.ConfigureAppConfiguration(configureAction);
            return this;
        }

        /// <summary>
        ///     Hook for configuring services.
        /// </summary>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        public SystemUnderTest ConfigureServices(Action<IServiceCollection> configureServices)
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

        /// <summary>
        ///     Create http client to access system under test.
        /// </summary>
        /// <returns>HttpClient</returns>
        public abstract HttpClient CreateClient();

        /// <summary>
        ///     Create http client to access system under test with options.
        /// </summary>
        /// <param name="options">FactoryOptions</param>
        /// <returns>HttpClient</returns>
        public abstract HttpClient CreateClient(WebApplicationFactoryClientOptions options);

        /// <summary>
        ///     Create http default client with delegate handlers.
        /// </summary>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public abstract HttpClient CreateDefaultClient(params DelegatingHandler[] handlers);

        /// <summary>
        ///     Create http default client with uri and delegate handlers.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public abstract HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers);

        public abstract void Dispose();
    }
}