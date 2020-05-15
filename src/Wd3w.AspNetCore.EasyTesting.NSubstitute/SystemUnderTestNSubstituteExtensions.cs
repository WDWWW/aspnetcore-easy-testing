using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Wd3w.AspNetCore.EasyTesting.NSubstitute
{
    /// <summary>
    ///     NSubstitute extension helper for SystemUnderTestBase
    /// </summary>
    public static class SystemUnderTestNSubstituteExtensions
    {
        /// <summary>
        ///     Replace original registry with NSubstitute object. If you call this replace method multiple times, the substitute is created only once.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="substitute">substitute</param>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest ReplaceWithNSubstitute<T>(this SystemUnderTest sut, out T substitute) where T : class
        {
            sut.CheckClientIsNotCreated(nameof(ReplaceWithNSubstitute));
            substitute = sut.GetOrAddInternalService(_ => Substitute.For<T>());
            sut.ReplaceService(substitute);
            return sut;
        }

        /// <summary>
        ///     Replace original registry with NSubstitute object. If you call this replace method multiple times, the substitute is created only once.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="setup">Setup substitute</param>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest ReplaceWithNSubstitute<T>(this SystemUnderTest sut, [NotNull] Action<T> setup) where T : class
        {
            sut.ReplaceWithNSubstitute<T>(out var substitute);
            setup(substitute);
            return sut;
        }

        /// <summary>
        ///     Get pre-registered NSubstitute object.
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns></returns>
        public static T GetSubstitute<T>(this SystemUnderTest sut)
        {
            return sut.InternalServiceProvider.GetService<T>() ?? throw new InvalidOperationException("Replace first using ReplaceWithNSubstitute before call UseSubstitute.");
        }

        /// <summary>
        ///     Get pre-registered NSubstitute object.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="substitute"></param>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest GetSubstitute<T>(this SystemUnderTest sut, out T substitute)
        {
            substitute = sut.GetSubstitute<T>();
            return sut;
        }

        /// <summary>
        ///     Use pre-registered NSubstitute object.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="verify"></param>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest UseSubstitute<T>(this SystemUnderTest sut, Action<T> verify)
        {
            var service = sut.GetSubstitute<T>();
            verify(service);
            return sut;
        }
    }
}