using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class GetOrAddInternalServiceTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_ReturnObject_When_TheServiceIsNotRegistered()
        {
            // Given
            // When
            var service = SUT.GetOrAddInternalService<ISampleService>(() => new FakeSampleService());

            // Then
            service.Should().NotBeNull();
        }

        [Fact]
        public void Should_ReturnSameObject_When_TheServiceIsAlreadyRegistered()
        {
            // Given
            var service = new FakeSampleService();
            SUT.InternalServiceCollection.AddSingleton<ISampleService>(service);

            // When
            var gettingService = SUT.GetOrAddInternalService<ISampleService>(() => new FakeSampleService());

            // Then
            service.Should().BeSameAs(gettingService);
        }

        [Fact]
        public void Should_ReturnSameObject_When_TheAddMethodIsCalledMultipleTimes()
        {
            // Given
            var preRegisteredServiceObject = SUT.GetOrAddInternalService(() => new FakeSampleService());

            // When
            var secondRegisteredService = SUT.GetOrAddInternalService(() => new FakeSampleService());

            // Then
            preRegisteredServiceObject.Should().BeSameAs(secondRegisteredService);
        }
    }
}