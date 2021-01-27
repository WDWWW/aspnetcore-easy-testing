using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wd3w.AspNetCore.EasyTesting.Internal;

[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.NSubstitute")]
[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.Moq")]
[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.FakeItEasy")]
[assembly: InternalsVisibleTo("Wd3w.AspNetCore.EasyTesting.Test")]

namespace Wd3w.AspNetCore.EasyTesting
{
    public abstract partial class SystemUnderTest : IDisposable
    {
        private IServiceCollection _serviceCollection;

        public IServiceProvider ServiceProvider { get; protected set; }

        internal IServiceCollection InternalServiceCollection { get; } = new ServiceCollection();

        internal IServiceProvider InternalServiceProvider => InternalServiceCollection.BuildServiceProvider();

        public abstract void Dispose();

        internal event SetupFixtureHandler OnSetupFixtures;

        internal event ConfigureTestServiceHandler OnConfigureTestServices;

        internal event ConfigureWebHostBuilderHandler OnConfigureWebHostBuilder;

        internal void CheckClientIsNotCreated(string methodName)
        {
            if (ServiceProvider != default)
                throw new InvalidOperationException($"{methodName} can be using before calling CreateClient/Create* methods.");
        }

        internal SystemUnderTest CheckClientIsNotCreated(string methodName, Action action)
        {
            CheckClientIsNotCreated(methodName);
            action();
            return this;
        }

        internal SystemUnderTest CheckClientIsNotCreated(string methodName, SetupFixtureHandler action)
        {
            CheckClientIsNotCreated(methodName);
            OnSetupFixtures += action;
            return this;
        }
        
        internal SystemUnderTest CheckClientIsNotCreatedAndConfigureServices(string methodName, ConfigureTestServiceHandler action)
        {
            CheckClientIsNotCreated(methodName);
            OnConfigureTestServices += action;
            return this;
        }
        
        internal SystemUnderTest CheckClientIsNotCreated(string methodName, ConfigureWebHostBuilderHandler action)
        {
            CheckClientIsNotCreated(methodName);
            OnConfigureWebHostBuilder += action;
            return this;
        }

        internal void CheckClientIsCreated(string methodName)
        {
            if (ServiceProvider == default) throw new InvalidOperationException($"{methodName} can be using after calling CreateClient/Create& methods.");
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
                var descriptor = services.FindServiceDescriptor<TService>();
                services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation),
                    lifetime ?? descriptor.Lifetime));
            };
            return this;
        }

        /// <summary>
        ///     Replace logger providers with new logger factory 
        /// </summary>
        /// <param name="providers"></param>
        /// <returns></returns>
        public SystemUnderTest ReplaceLoggerFactory(params ILoggerProvider[] providers)
        {
            CheckClientIsNotCreated(nameof(ReplaceLoggerFactory));
            ReplaceService<ILoggerFactory>(new LoggerFactory(providers));
            return this;
        }

        /// <summary>
        ///     Replace logger factory with logger builder
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public SystemUnderTest ReplaceLoggerFactory(Action<ILoggingBuilder> configure)
        {
            CheckClientIsNotCreated(nameof(ReplaceLoggerFactory));
            ReplaceService(LoggerFactory.Create(configure));
            return this;
        }


        /// <summary>
        ///     Replace distributed cache to in-memory cache for testing.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public SystemUnderTest ReplaceDistributedInMemoryCache(MemoryDistributedCacheOptions options = default)
        {
            ReplaceService<IDistributedCache>(new MemoryDistributedCache(
                new OptionsWrapper<MemoryDistributedCacheOptions>(options ?? new MemoryDistributedCacheOptions())));
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
            OnConfigureTestServices += services => services.Replace(new ServiceDescriptor(typeof(TService), obj));
            return this;
        }

        /// <summary>
        ///     Replace registered configure options.
        /// </summary>
        /// <param name="configurer">Option configurer</param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public SystemUnderTest ReplaceConfigureOptions<TOptions>(Action<TOptions> configurer) where TOptions : class
        {
            CheckClientIsNotCreated(nameof(ReplaceConfigureOptions));
            OnConfigureTestServices += services =>
            {
                services.RemoveAll<IConfigureOptions<TOptions>>();
                services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureOptions<TOptions>(configurer));
            };
            return this;
        }

        /// <summary>
        ///     Remove all TService service registrations.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public SystemUnderTest RemoveAll<TService>()
        {
            return CheckClientIsNotCreatedAndConfigureServices(nameof(RemoveAll), services => services.RemoveAll<TService>());
        }

        /// <summary>
        ///     Remove all TService service registrations.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public SystemUnderTest RemoveAll(Type serviceType)
        {
            return CheckClientIsNotCreatedAndConfigureServices(nameof(RemoveAll), services => services.RemoveAll(serviceType));
        }

        /// <summary>
        ///     Remove TImplementation of TService registrations.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public SystemUnderTest Remove<TService, TImplementation>()
        {
            return Remove(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        ///     Remove implementationType of serviceType registrations.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public SystemUnderTest Remove(Type serviceType, Type implementationType)
        {
            return CheckClientIsNotCreatedAndConfigureServices(nameof(Remove), services =>
            {
                var descriptor = services.FirstOrDefault(registration =>
                    registration.ServiceType == serviceType &&
                    registration.ImplementationType == implementationType);
                if (descriptor == default) return;
                services.Remove(descriptor);
            });
        }

        /// <summary>
        ///     Remove registration by condition expression.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SystemUnderTest RemoveAllBy(Func<ServiceDescriptor, bool> condition)
        {
            return CheckClientIsNotCreatedAndConfigureServices(nameof(RemoveAllBy), services =>
            {
                foreach (var descriptor in services.Where(condition).ToArray())
                {
                    services.Remove(descriptor);
                }
            });
        }
        
        /// <summary>
        ///     Remove only one registration by condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SystemUnderTest RemoveSingleBy(Func<ServiceDescriptor, bool> condition)
        {
            return CheckClientIsNotCreatedAndConfigureServices(nameof(RemoveSingleBy), services => services.Remove(services.Single(condition)));
        }

        /// <summary>
        ///     Remove all startup filters.
        /// </summary>
        /// <returns></returns>
        public SystemUnderTest DisableStartupFilters()
        {
            return CheckClientIsNotCreatedAndConfigureServices(nameof(DisableStartupFilters), services => services.RemoveAll<IStartupFilter>());
        }

        /// <summary>
        ///     Remove specific startup filter for TImplementationFilter
        /// </summary>
        /// <typeparam name="TImplementationFilter"></typeparam>
        /// <returns></returns>
        public SystemUnderTest DisableStartupFilter<TImplementationFilter>()
            where TImplementationFilter : IStartupFilter
        {
            var filterType = typeof(TImplementationFilter);
            return CheckClientIsNotCreatedAndConfigureServices(nameof(DisableStartupFilter), services =>
            {
                var registration = services.First(descriptor => descriptor.ImplementationType == filterType || descriptor.ImplementationInstance?.GetType() == filterType);
                services.Remove(registration);
            });
        }

        /// <summary>
        ///     Replace registered named options configurer
        /// </summary>
        /// <param name="name">Option name</param>
        /// <param name="configurer"></param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public SystemUnderTest ReplaceNamedConfigureOptions<TOptions>(string name, Action<TOptions> configurer) where TOptions : class
        {
            CheckClientIsNotCreated(nameof(ReplaceConfigureOptions));
            OnConfigureTestServices += services =>
            {
                foreach (var remove in services.Where(descriptor => descriptor.ServiceType == typeof(IConfigureOptions<TOptions>))
                    .Where(descriptor => descriptor.ImplementationInstance is ConfigureNamedOptions<TOptions> configure && configure.Name == name)
                    .ToList())
                {
                    services.Remove(remove);
                }

                services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureNamedOptions<TOptions>(name, configurer));
            };
            return this;
        }
        
        /// <summary>
        ///     Remove registered IValidateOptions for TOptions
        /// </summary>
        /// <typeparam name="TOptions">Option class</typeparam>
        /// <returns></returns>
        public SystemUnderTest DisableOptionValidations<TOptions>() where TOptions : class
        {
            CheckClientIsNotCreated(nameof(DisableOptionValidations));
            OnConfigureTestServices += services => services.RemoveAll<IValidateOptions<TOptions>>();
            return this;
        }

        /// <summary>
        ///     Remove data annotation validator only for TOption
        /// </summary>
        /// <typeparam name="TOptions">The option class</typeparam>
        /// <returns></returns>
        public SystemUnderTest DisableOptionDataAnnotationValidation<TOptions>() where TOptions : class
        {
            CheckClientIsNotCreated(nameof(DisableOptionDataAnnotationValidation));
            OnConfigureTestServices += services =>
            {
                var serviceDescriptor = services.FirstOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IValidateOptions<TOptions>) &&
                    descriptor.ImplementationInstance != null &&
                    descriptor.ImplementationInstance.GetType() == typeof(DataAnnotationValidateOptions<TOptions>));

                if (serviceDescriptor != default)
                    services.Remove(serviceDescriptor);
            };
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

        /// <summary>
        ///     Use services from internal service providers
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task UsingServiceAsync(Func<IServiceProvider, Task> action)
        {
            CheckClientIsCreated(nameof(UsingServiceAsync));
            using var scope = ServiceProvider.CreateScope();
            await action.Invoke(scope.ServiceProvider);
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
            using var scope = ServiceProvider.CreateScope();
            await action.Invoke(scope.ServiceProvider.GetService<TService>());
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
            using var scope = ServiceProvider.CreateScope();
            await action.Invoke(scope.ServiceProvider.GetService<TService1>(),
                scope.ServiceProvider.GetService<TService2>());
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
            using var scope = ServiceProvider.CreateScope();
            var provider = scope.ServiceProvider;
            await action.Invoke(provider.GetService<TService1>(), provider.GetService<TService2>(),
                provider.GetService<TService3>());
        }

        /// <summary>
        ///     Use service provider.
        /// </summary>
        /// <param name="action"></param>
        public void UsingService(Action<IServiceProvider> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using var scope = ServiceProvider.CreateScope();
            action.Invoke(scope.ServiceProvider);
        }

        /// <summary>
        ///     Use TService object from system service provider.
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TService"></typeparam>
        public void UsingService<TService>(Action<TService> action)
        {
            CheckClientIsCreated(nameof(UsingService));
            using var scope = ServiceProvider.CreateScope();
            action.Invoke(scope.ServiceProvider.GetService<TService>());
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
            using var scope = ServiceProvider.CreateScope();
            var provider = scope.ServiceProvider;
            action.Invoke(provider.GetService<TService1>(), provider.GetService<TService2>());
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
            using var scope = ServiceProvider.CreateScope();
            var provider = scope.ServiceProvider;
            action.Invoke(provider.GetService<TService1>(), ServiceProvider.GetService<TService2>(),
                ServiceProvider.GetService<TService3>());
        }

        private ServiceDescriptor FindServiceDescriptor<TService>()
        {
            return _serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(TService))
                   ?? throw new InvalidOperationException(
                       "The provided service type is not registered from SUT service collection.");
        }

        private Type GetImplementationType(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != default)
                return descriptor.ImplementationType;
            
            if (descriptor.ImplementationInstance != default)
                return descriptor.ImplementationInstance.GetType();


            using var provider = _serviceCollection.BuildServiceProvider();
            using var serviceScope = provider.CreateScope();
            var implementationTypeObject = serviceScope.ServiceProvider.GetService(descriptor.ServiceType);
            if (implementationTypeObject == null)
                throw new InvalidOperationException(
                    "Can not get type of service type from implementation factory of service descriptor in service collection.");

            return implementationTypeObject.GetType();
        }

        private void CheckServiceCollectionAllocated()
        {
            if (_serviceCollection == default)
                throw new InvalidOperationException(
                    "Should create client before verify service's registered lifetime.");
        }

        /// <summary>
        ///     Setup configure web host builder.
        /// </summary>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public SystemUnderTest SetupWebHostBuilder(Action<IWebHostBuilder> configureAction)
        {
            CheckClientIsNotCreated(nameof(SetupWebHostBuilder));
            OnConfigureWebHostBuilder += configureAction.Invoke;
            return this;
        }

        /// <summary>
        ///     UseEnvironment delegate method.
        /// </summary>
        /// <param name="environment">Specific environment for starting host</param>
        /// <returns></returns>
        public SystemUnderTest UseEnvironment(string environment)
        {
            CheckClientIsNotCreated(nameof(UseEnvironment));
            OnConfigureWebHostBuilder += builder => builder.UseEnvironment(environment);
            return this;
        }

        /// <summary>
        ///     Use Production environment when host is started.
        /// </summary>
        /// <returns></returns>
        public SystemUnderTest UseProductionEnvironment()
        {
            CheckClientIsNotCreated(nameof(UseProductionEnvironment));
            OnConfigureWebHostBuilder += builder => builder.UseEnvironment(Environments.Production);
            return this;
        }

        /// <summary>
        ///     Use Staging environment when host is started.
        /// </summary>
        /// <returns></returns>
        public SystemUnderTest UseStagingEnvironment()
        {
            CheckClientIsNotCreated(nameof(UseStagingEnvironment));
            OnConfigureWebHostBuilder += builder => builder.UseEnvironment(Environments.Staging);
            return this;
        }

        /// <summary>
        ///     Use Development environment when host is started.
        /// </summary>
        /// <returns></returns>
        public SystemUnderTest UseDevelopmentEnvironment()
        {
            CheckClientIsNotCreated(nameof(UseDevelopmentEnvironment));
            OnConfigureWebHostBuilder += builder => builder.UseEnvironment(Environments.Development);
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
            CheckClientIsNotCreated(nameof(UseSetting));
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
            CheckClientIsNotCreated(nameof(ConfigureAppConfiguration));
            OnConfigureWebHostBuilder += builder => builder.ConfigureAppConfiguration(configureAction);
            return this;
        }

        /// <summary>
        ///     Override app configuration members with options instance
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SystemUnderTest OverrideAppConfiguration<T>(T options) where T : class
        {
            CheckClientIsNotCreated(nameof(OverrideAppConfiguration));
            var json = JsonSerializer.SerializeToUtf8Bytes(options);
            ConfigureAppConfiguration(builder => builder.AddJsonStream(new MemoryStream(json)));
            return this;
        }

        /// <summary>
        ///     Override app configuration members with dynamic instance
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public SystemUnderTest OverrideAppConfiguration(dynamic options)
        {
            CheckClientIsNotCreated(nameof(OverrideAppConfiguration));
            var json = JsonSerializer.SerializeToUtf8Bytes(options);
            ConfigureAppConfiguration(builder => builder.AddJsonStream(new MemoryStream(json)));
            return this;
        }

        /// <summary>
        ///     Override app configuration members with path
        /// </summary>
        /// <param name="configurationPath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SystemUnderTest OverrideAppConfiguration(string configurationPath, string value)
        {
            CheckClientIsNotCreated(nameof(OverrideAppConfiguration));
            ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new []{new KeyValuePair<string, string>(configurationPath, value)}));
            return this;
        }

        /// <summary>
        ///     Override app configuration members with key value collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public SystemUnderTest OverrideAppConfiguration(IDictionary<string, string> collection)
        {
            CheckClientIsNotCreated(nameof(OverrideAppConfiguration));
            ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(collection));
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
            });
        }

        internal void ExecuteSetupFixture()
        {
            using var scope = ServiceProvider.CreateScope();
            Task.WhenAll(OnSetupFixtures
                    ?.GetInvocationList()
                    .Cast<SetupFixtureHandler>()
                    .Select(handler => handler(scope.ServiceProvider)))
                .Wait();
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

        internal delegate void ConfigureTestServiceHandler(IServiceCollection services);

        internal delegate Task SetupFixtureHandler(IServiceProvider provider);

        internal delegate void ConfigureWebHostBuilderHandler(IWebHostBuilder builder);
    }
}