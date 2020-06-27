using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Wd3w.AspNetCore.EasyTesting.Authentication
{
    public static class AuthenticationHandlerHelper
    {
        private static Action<AuthenticationOptions> GenerateSchemeNameValidator(string schemeName)
        {
            return options =>
            {
                if (!options.Schemes.Select(s => s.Name).Contains(schemeName))
                    throw new InvalidOperationException($"There is no registered scheme({schemeName}).");
            };
        }
        
        public static SystemUnderTest FakeAuthentication(this SystemUnderTest sut,
            string scheme,
            AuthenticateResult result)
        {
            return sut.FakeAuthentication((options, builder) => builder.Name == scheme, 
                result, 
                GenerateSchemeNameValidator(scheme));
        }

        public static SystemUnderTest FakeAuthentication(this SystemUnderTest sut, AuthenticateResult result)
        {
            return sut.FakeAuthentication((options, builder) => options.DefaultScheme == builder.Name, result);
        }

        public static SystemUnderTest ReplaceAuthenticationHandler<THandler>(this SystemUnderTest sut,
            string scheme,
            ServiceLifetime lifetime)
            where THandler : IAuthenticationHandler
        {
            return sut.ReplaceAuthenticationHandler((_, builder) => builder.Name == scheme, 
                (_, builder) => new ServiceDescriptor(typeof(THandler), typeof(THandler), lifetime),
                validateOptions: GenerateSchemeNameValidator(scheme));
        }

        public static SystemUnderTest ReplaceAuthenticationHandler<THandler>(this SystemUnderTest sut, 
            string scheme,
            THandler handler)
            where THandler : IAuthenticationHandler
        {
            return sut.ReplaceAuthenticationHandler((_, builder) => builder.Name == scheme,
                (_, builder) => new ServiceDescriptor(typeof(THandler), handler),
                validateOptions: GenerateSchemeNameValidator(scheme));
        }

        public static SystemUnderTest ReplaceAuthenticationHandler<THandler>(this SystemUnderTest sut,
            string scheme,
            Func<IServiceProvider, THandler> factory,
            ServiceLifetime lifetime)
            where THandler : IAuthenticationHandler
        {
            return sut.ReplaceAuthenticationHandler((_, builder) => builder.Name == scheme,
                (_, builder) => new ServiceDescriptor(typeof(THandler), provider => factory(provider), lifetime),
                validateOptions: GenerateSchemeNameValidator(scheme));
        }

        internal static SystemUnderTest FakeAuthentication(this SystemUnderTest sut,
            Func<AuthenticationOptions, AuthenticationSchemeBuilder, bool> schemePicker,
            AuthenticateResult result, 
            Action<AuthenticationOptions> validateOptions = default)
        {
            static Type MakeFakeHandlerType(AuthenticationSchemeBuilder builder)
            {
                if (builder.HandlerType.IsGenericType && builder.HandlerType.GetGenericTypeDefinition() == typeof(FakeAuthenticationHandler<>))
                    return builder.HandlerType;
                
                var baseType = builder
                    .HandlerType
                    .BaseType;
                
                if (baseType!.GetGenericTypeDefinition() != typeof(AuthenticationHandler<>))
                    throw new InvalidOperationException("Target scheme authentication handler must inherited AuthenticationHandler<TOptions>");
                    
                var options = baseType
                    .GetGenericArguments()
                    .First();
                return typeof(FakeAuthenticationHandler<>).MakeGenericType(options);
            }
            if (sut.ServiceProvider == null)
            {
                sut.ReplaceAuthenticationHandler(schemePicker, 
                    (options, builder) =>
                    {
                        var fakeHandler = MakeFakeHandlerType(builder);
                        return new ServiceDescriptor(fakeHandler, fakeHandler, ServiceLifetime.Singleton);
                    }, 
                    (services, type) =>
                    {
                        using var buildInstanceProvider = services.BuildServiceProvider();
                        var handler = (IFakeAuthenticationHandler)buildInstanceProvider.GetService(type);
                        handler.SetResult(result);
                        services.RemoveAll(type);
                        services.AddSingleton(type, handler);
                    },
                    validateOptions);
            }
            else
            {
                sut.UsingService(provider =>
                {
                    var options = provider.GetService<IOptions<AuthenticationOptions>>()?.Value
                                  ?? throw new InvalidOperationException(
                                      "Couldn't find AuthenticationOptions from SUT. Please configure your own authentication.");

                    var builder = options.Schemes.First(scheme => schemePicker(options, scheme));
                    var fakeHandlerType = MakeFakeHandlerType(builder);
                    var fakeHandler = (IFakeAuthenticationHandler) provider.GetService(fakeHandlerType) 
                                      ?? throw new InvalidOperationException("Couldn't find fake handler. Please call *Authentication before calling Fake/NoUser/Deny/AllowAuthentication");
                    fakeHandler.SetResult(result);
                });
            }
            return sut;
        }

        internal static SystemUnderTest ReplaceAuthenticationHandler(this SystemUnderTest sut, 
            Func<AuthenticationOptions, AuthenticationSchemeBuilder, bool> schemePicker,
            Func<AuthenticationOptions, AuthenticationSchemeBuilder, ServiceDescriptor> handlerType,
            Action<IServiceCollection, Type> postProcess = default,
            Action<AuthenticationOptions> validateOptions = default)
        {
            sut.CheckClientIsNotCreated(nameof(ReplaceAuthenticationHandler));
            
            // see https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication/AuthenticationBuilder.cs
            sut.OnConfigureTestServices += services =>
            {
                if (services.All(sd => sd.ServiceType != typeof(IConfigureOptions<AuthenticationOptions>)))
                    throw new InvalidOperationException(
                        "Couldn't find AuthenticationOptions. You should add authentication scheme at least one on your startup class.");
                
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    validateOptions?.Invoke(options);
                    foreach (var builder in options.Schemes)
                    {
                        if (!schemePicker(options, builder))
                            continue;
                    
                        services.RemoveAll(builder.HandlerType);
                        var serviceDescriptor = handlerType(options, builder);
                        builder.HandlerType = serviceDescriptor.ServiceType;
                        services.Add(serviceDescriptor);
                        postProcess?.Invoke(services, builder.HandlerType);
                    }
                });
            };

            return sut;
        }
    }
}