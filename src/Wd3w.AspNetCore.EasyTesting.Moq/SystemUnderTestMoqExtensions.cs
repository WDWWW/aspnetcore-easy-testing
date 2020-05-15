using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Wd3w.AspNetCore.EasyTesting.Moq
{
    /// <summary>
    ///     Mock extension helper for SystemUnderTestBase
    /// </summary>
    public static class SystemUnderTestMoqExtensions
    {
        /// <summary>
        ///     Replace TService with Mock.Object on ASP.NET Core internal service. If you call this method multiple time,
        ///     It's only replace the service once.
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="TService">Service type to mock</typeparam>
        /// <returns></returns>
        public static Mock<TService> MockService<TService>(this SystemUnderTest sut) where TService : class
        {
            sut.CheckClientIsNotCreated(nameof(MockService));
            var mock = sut.GetOrAddInternalService(() => new Mock<TService>());
            sut.ReplaceService(mock.Object);
            return mock;
        }

        /// <summary>
        ///     Replace TService with Mock.Object on ASP.NET Core internal service. If you call this method multiple time,
        ///     It's only replace the service once.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="mock">mock object</param>
        /// <typeparam name="TService">Service type to mock</typeparam>
        /// <returns></returns>
        public static SystemUnderTest MockService<TService>(this SystemUnderTest sut, out Mock<TService> mock) where TService : class
        {
            mock = sut.MockService<TService>();
            return sut;
        }

        /// <summary>
        ///     Replace TService with Mock.Object on ASP.NET Core internal service. If you call this method multiple time,
        ///     It's only replace the service once.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="mockAction">action to configure mock object</param>
        /// <typeparam name="TService">Service type to mock</typeparam>
        /// <returns></returns>
        public static SystemUnderTest MockService<TService>(this SystemUnderTest sut, Action<Mock<TService>> mockAction) where TService : class
        {
            mockAction(sut.MockService<TService>());
            return sut;
        }

        /// <summary>
        ///     Get service mock object from internal service collection. You should mock service before call this method.
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="TService">Mocked service type</typeparam>
        /// <returns></returns>
        public static Mock<TService> GetServiceMock<TService>(this SystemUnderTest sut) where TService : class
        {
            return sut.InternalServiceProvider.GetService<Mock<TService>>()
                ?? throw new InvalidOperationException("Replace service to mock first using ReplaceWithNSubstitute before call this method.");
        }

        /// <summary>
        ///     Get service mock object from internal service collection. You should mock service before call this method.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="mock">mock object</param>
        /// <typeparam name="TService">Mocked service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest GetServiceMock<TService>(this SystemUnderTest sut, out Mock<TService> mock)
            where TService : class
        {
            mock = sut.GetServiceMock<TService>();
            return sut;
        }

        /// <summary>
        ///     Use service mock object from internal service collection. You should mock service before call this method.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="useMock">use mock</param>
        /// <typeparam name="TService">Mocked service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest UseServiceMock<TService>(this SystemUnderTest sut, Action<Mock<TService>> useMock)
            where TService : class
        {
            useMock(sut.GetServiceMock<TService>());
            return sut;
        }

        /// <summary>
        ///     Verify mock service short method.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="expression">Expression for verify</param>
        /// <param name="times">Times for verifying. Default value is AtLeastOnce if the times is not provided.</param>
        /// <typeparam name="TService">Mocked service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest VerifyCall<TService>(this SystemUnderTest sut,
            Expression<Action<TService>> expression,
            Times? times = null)
            where TService : class
        {
            sut.UseServiceMock<TService>(mock => mock.Verify(expression, times ?? Times.AtLeastOnce()));
            return sut;
        }

        /// <summary>
        ///     Verify mock service short method.
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="expression">Expression for verify</param>
        /// <typeparam name="TService">Mocked service type</typeparam>
        /// <returns></returns>
        public static SystemUnderTest VerifyCallOnce<TService>(this SystemUnderTest sut,
            Expression<Action<TService>> expression)
            where TService : class
        {
            sut.UseServiceMock<TService>(mock => mock.Verify(expression, Times.Once));
            return sut;
        }
    }
}