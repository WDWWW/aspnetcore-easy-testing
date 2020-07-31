using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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
                schemeName ??= options.DefaultScheme;
                if (!options.Schemes.Select(s => s.Name).Contains(schemeName))
                    throw new InvalidOperationException($"There is no registered scheme({schemeName}).");
            };
        }

        public static SystemUnderTest NoUserAuthentication(this SystemUnderTest sut)
        {
            return sut.FakeAuthentication(AuthenticateResult.NoResult());
        }

        public static SystemUnderTest NoUserAuthentication(this SystemUnderTest sut, string scheme)
        {
            return sut.FakeAuthentication(scheme, AuthenticateResult.NoResult());
        }

        
        public static SystemUnderTest DenyAuthentication(this SystemUnderTest sut, string message = "Failed to authentication")
        {
            return sut.FakeAuthentication(AuthenticateResult.Fail(message));
        }
        
        public static SystemUnderTest DenyAuthentication(this SystemUnderTest sut, string scheme, string message = "Failed to authentication")
        {
            return sut.FakeAuthentication(scheme, AuthenticateResult.Fail(message));
        }
        
        public static SystemUnderTest DenyAuthentication(this SystemUnderTest sut, Exception exception)
        {
            return sut.FakeAuthentication(AuthenticateResult.Fail(exception));
        }
        
        public static SystemUnderTest DenyAuthentication(this SystemUnderTest sut, string scheme, Exception exception)
        {
            return sut.FakeAuthentication(scheme, AuthenticateResult.Fail(exception));
        }
        
        public static SystemUnderTest AllowAuthentication(this SystemUnderTest sut, params Claim[] identity)
        {
            return sut.FakeAuthenticationCore(options => AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(identity, options.DefaultScheme)), options.DefaultScheme)));
        }


        public static SystemUnderTest AllowAuthentication(this SystemUnderTest sut, IIdentity identity)
        {
            CheckClaimsIsAuthenticated(identity);
            return sut.FakeAuthenticationCore(options => AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), options.DefaultScheme)));
        }

        
        public static SystemUnderTest AllowAuthentication(this SystemUnderTest sut, ClaimsPrincipal pricipal)
        {
            CheckClaimsIsAuthenticated(pricipal.Identity);
            return sut.FakeAuthenticationCore(options => AuthenticateResult.Success(new AuthenticationTicket(pricipal, options.DefaultScheme)));
        }
        
        public static SystemUnderTest AllowAuthentication(this SystemUnderTest sut, string scheme, params Claim[] identity)
        {
            return sut.FakeAuthentication(scheme, AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(identity, scheme)), scheme)));
        }


        public static SystemUnderTest AllowAuthentication(this SystemUnderTest sut, string scheme, IIdentity identity)
        {
            CheckClaimsIsAuthenticated(identity);
            return sut.FakeAuthentication(scheme, AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), scheme)));
        }

        
        public static SystemUnderTest AllowAuthentication(this SystemUnderTest sut, string scheme, ClaimsPrincipal pricipal)
        {
            CheckClaimsIsAuthenticated(pricipal.Identity);
            return sut.FakeAuthentication(scheme, AuthenticateResult.Success(new AuthenticationTicket(pricipal, scheme)));
        }

        private static void CheckClaimsIsAuthenticated(IIdentity identity)
        {
            if (!identity.IsAuthenticated)
                throw new ArgumentException(
                    "IIdentity.IsAuthenticated should be true. If you create new instance of ClaimsIdentity, you should create instance with AuthenticationType param of constructor.");
        }


        public static SystemUnderTest FakeAuthentication(this SystemUnderTest sut, AuthenticateResult result)
        {
            return sut.FakeAuthentication(default, result);
        }

        public static SystemUnderTest FakeAuthentication(this SystemUnderTest sut,
            string scheme,
            AuthenticateResult result)
        {
            if (result.Succeeded)
                CheckClaimsIsAuthenticated(result.Principal.Identity);
            
            return sut.FakeAuthenticationCore(scheme, 
                _ => result, 
                GenerateSchemeNameValidator(scheme));
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
        
        internal static SystemUnderTest FakeAuthenticationCore(this SystemUnderTest sut, Func<AuthenticationOptions, AuthenticateResult> resultCreator)
        {
            return sut.FakeAuthenticationCore(default,
                options =>
                {
                    var authenticateResult = resultCreator(options);
                    if (authenticateResult.Succeeded)
                        CheckClaimsIsAuthenticated(authenticateResult.Principal.Identity);

                    return authenticateResult;
                });
        }

        /// <summary>
        ///     Fake authentication core method
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="schemeName"></param>
        /// <param name="result"></param>
        /// <param name="validateOptions"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal static SystemUnderTest FakeAuthenticationCore(this SystemUnderTest sut,
            string schemeName,
            Func<AuthenticationOptions, AuthenticateResult> result, 
            Action<AuthenticationOptions> validateOptions = default)
        {
            static Type MakeFakeHandlerType(AuthenticationSchemeBuilder builder)
            {
                if (IsFakeAuthenticationHandler(builder.HandlerType))
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
                sut.ReplaceAuthenticationHandler((options, builder) => schemeName != null 
                        ? builder.Name == schemeName 
                        : options.DefaultScheme == builder.Name, 
                    (options, builder) =>
                    {
                        var fakeHandler = MakeFakeHandlerType(builder);
                        return new ServiceDescriptor(fakeHandler, fakeHandler, ServiceLifetime.Singleton);
                    }, 
                    (services, type, options) =>
                    {
                        using var buildInstanceProvider = services.BuildServiceProvider();
                        var handler = (IFakeAuthenticationHandler)buildInstanceProvider.GetService(type);
                        handler.SetResult(result(options));
                        services.RemoveAll(type);
                        services.AddSingleton(type, handler);
                    },
                    validateOptions);
            }
            else
            {
                sut.UsingServiceAsync(async provider =>
                {
                    var authenticationOptions = provider.GetService<IOptions<AuthenticationOptions>>()?.Value;
                    if (authenticationOptions == null)
                        throw new InvalidOperationException(
                            "Couldn't find AuthenticationOptions from SUT. Please configure your own authentication.");

                    validateOptions?.Invoke(authenticationOptions);

                    var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
                    var schemes = schemeName == default
                        ? await schemeProvider.GetDefaultAuthenticateSchemeAsync()
                        : await schemeProvider.GetSchemeAsync(schemeName);

                    if (!IsFakeAuthenticationHandler(schemes.HandlerType))
                        throw new InvalidOperationException(
                            "Couldn't find fake handler. Please call *Authentication before calling Fake/NoUser/Deny/AllowAuthentication");
                    var fakeHandler = (IFakeAuthenticationHandler) provider.GetService(schemes.HandlerType);
                    fakeHandler.SetResult(result(authenticationOptions));
                }).Wait();
            }
            return sut;
        }

        private static bool IsFakeAuthenticationHandler(Type handlerType)
        {
            return handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(FakeAuthenticationHandler<>);
        }

        internal static SystemUnderTest ReplaceAuthenticationHandler(this SystemUnderTest sut, 
            Func<AuthenticationOptions, AuthenticationSchemeBuilder, bool> schemePicker,
            Func<AuthenticationOptions, AuthenticationSchemeBuilder, ServiceDescriptor> handlerType,
            Action<IServiceCollection, Type, AuthenticationOptions> postProcess = default,
            Action<AuthenticationOptions> validateOptions = default)
        {
            sut.CheckClientIsNotCreated(nameof(ReplaceAuthenticationHandler));
            
            // see https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication/AuthenticationBuilder.cs
            sut.OnConfigureTestServices += services =>
            {
                if (services.All(sd => sd.ServiceType != typeof(IConfigureOptions<AuthenticationOptions>)))
                    throw new InvalidOperationException(
                        "Couldn't find AuthenticationOptions. You should add authentication scheme at least one on your startup class.");

                using var provider = services.BuildServiceProvider();
                var options = provider.GetService<IOptions<AuthenticationOptions>>().Value;
                
                foreach (var builder in options.Schemes)
                {
                    if (!schemePicker(options, builder))
                        continue;
                    validateOptions?.Invoke(options);
                    services.RemoveAll(builder.HandlerType);
                    services.Add(handlerType(options, builder));
                }

                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    validateOptions?.Invoke(options);
                    foreach (var builder in options.Schemes)
                    {
                        if (!schemePicker(options, builder))
                            continue;

                        builder.HandlerType = handlerType(options, builder).ServiceType;
                        postProcess?.Invoke(services, builder.HandlerType, options);
                    }
                });
            };

            return sut;
        }
    }
}