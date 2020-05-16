using System;
using System.Diagnostics.CodeAnalysis;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

namespace Wd3w.AspNetCore.EasyTesting.FakeItEasy
{
    /// <summary>
    ///     FakeItEasy Extension for SystemUnderTest
    /// </summary>
    public static class SystemUnderTestFakeItEasyExtensions
    {
        /// <summary>
        ///     Replace service with dummy object
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="TService">Service to fake</typeparam>
        /// <returns></returns>
        public static SystemUnderTest ReplaceDummyService<TService>(this SystemUnderTest sut) where TService : class
        {
            sut.CheckClientIsNotCreated(nameof(FakeService));
            sut.ReplaceService(sut.GetOrAddInternalService(A.Dummy<TService>));
            return sut;
        }

        /// <summary>
        ///     Replace service with fake. If you call this replace method multiple times, The fake is created only once.
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="TService">Service to fake</typeparam>
        /// <returns></returns>
        public static TService FakeService<TService>(this SystemUnderTest sut) where TService : class
        {
            sut.CheckClientIsNotCreated(nameof(FakeService));
            var fake = sut.GetOrAddInternalService(A.Fake<TService>);
            sut.ReplaceService(fake);
            return fake;
        }

        /// <summary>
        ///     Replace service with fake. If you call this replace method multiple times, The fake is created only once.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="fake"></param>
        /// <typeparam name="TService">Service to fake</typeparam>
        /// <returns></returns>
        public static SystemUnderTest FakeService<TService>(this SystemUnderTest sut, [NotNull] out TService fake) where TService : class
        {
            fake = sut.FakeService<TService>();
            return sut;
        }

        /// <summary>
        ///     Replace service with fake. If you call this replace method multiple times, The fake is created only once.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="configureFake"></param>
        /// <typeparam name="TService">Service to fake</typeparam>
        /// <returns></returns>
        public static SystemUnderTest FakeService<TService>(this SystemUnderTest sut, [NotNull] Action<TService> configureFake) where TService : class
        {
            configureFake(sut.FakeService<TService>());
            return sut;
        }

        /// <summary>
        ///     Get pre-registered fake service object.
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="TService">Service to fake</typeparam>
        /// <returns></returns>
        public static TService GetFakeService<TService>(this SystemUnderTest sut) where TService : class
        {
            return sut.InternalServiceProvider.GetService<TService>() ?? throw new InvalidOperationException("Replace first using FakeService before call GetFakeService.");
        }

        /// <summary>
        ///     Use pre-registered fake service object.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="useFakeService"></param>
        /// <typeparam name="TService">Service to fake</typeparam>
        /// <returns></returns>
        public static SystemUnderTest UseFakeService<TService>(this SystemUnderTest sut, [NotNull] Action<TService> useFakeService) where TService : class
        {
            useFakeService(sut.GetFakeService<TService>());
            return sut;
        }
    }
}