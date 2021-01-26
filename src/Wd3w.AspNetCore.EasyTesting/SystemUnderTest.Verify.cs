using System;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Wd3w.AspNetCore.EasyTesting
{
    public abstract partial class SystemUnderTest
    {
        /// <summary>
        ///     Verify lifetime of registered service.
        /// </summary>
        /// <param name="lifetime"></param>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void VerifyRegisteredLifeTimeOfService<TService>(ServiceLifetime lifetime)
        {
            CheckServiceCollectionAllocated();
            FindServiceDescriptor<TService>().Lifetime.Should().Be(lifetime);
        }

        /// <summary>
        ///     Verify implementation type of registered service.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void VerifyRegisteredImplementationTypeOfService<TService, TImplementation>()
        {
            CheckServiceCollectionAllocated();
            GetImplementationType(FindServiceDescriptor<TService>()).Should().Be(typeof(TImplementation));
        }

        /// <summary>
        ///     Verify service collection with custom condition expression
        /// </summary>
        /// <param name="condition"></param>
        public void VerifyRegistrationByCondition(Expression<Func<ServiceDescriptor, bool>> condition)
        {
            CheckServiceCollectionAllocated();
            _serviceCollection.Should().Contain(condition);
        }

        /// <summary>
        ///     Verify there are no service descriptor registration by condition expression.
        /// </summary>
        /// <param name="condition"></param>
        public void VerifyNoRegistrationByCondition(Expression<Func<ServiceDescriptor, bool>> condition)
        {
            CheckServiceCollectionAllocated();
            _serviceCollection.Should().NotContain(condition);
        }

        /// <summary>
        ///     Verify there are no service descriptor of TService type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public void VerifyNoRegistration<TService>()
        {
            VerifyNoRegistration(typeof(TService));
        }

        /// <summary>
        ///     Verify there are no service descriptor of serviceType
        /// </summary>
        /// <param name="serviceType"></param>
        public void VerifyNoRegistration(Type serviceType)
        {
            CheckServiceCollectionAllocated();
            _serviceCollection.Should().NotContain(descriptor => descriptor.ServiceType == serviceType);
        }

        /// <summary>
        ///     Verify there are no service descriptor of TService type and TImplementation type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void VerifyNoRegistration<TService, TImplementation>() where TImplementation : class, TService
        {
            VerifyNoRegistration(typeof(TService), typeof(TImplementation));
        }


        /// <summary>
        ///     Verify there are no service descriptor of serviceType and implementationType
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public void VerifyNoRegistration(Type serviceType, Type implementationType)
        {
            CheckServiceCollectionAllocated();
            _serviceCollection.Should().NotContain(descriptor =>
                descriptor.ServiceType == serviceType && (descriptor.ImplementationType == implementationType ||
                                                          descriptor.ImplementationInstance.GetType() == implementationType));
        }
    }
}