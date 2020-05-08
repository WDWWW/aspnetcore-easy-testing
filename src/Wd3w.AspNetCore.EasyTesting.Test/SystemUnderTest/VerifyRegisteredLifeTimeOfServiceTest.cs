using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class VerifyRegisteredLifeTimeOfServiceTest : SystemUnderTestBase
    {
        [Fact]
        public void Should_ReturnTrue_When_SampleServiceIsRegisteredAsScoped()
        {
            // Given
            SUT.CreateClient();

            // When
            var result = SUT.VerifyRegisteredLifeTimeOfService<ISampleService>(ServiceLifetime.Scoped);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_SampleServiceIsRegistredSingleTonAndReplaceServiceWithSingleton()
        {
            // Given
            SUT.ReplaceService<ISampleService>(new SampleService())
                .CreateClient();

            // When
            var result = SUT.VerifyRegisteredLifeTimeOfService<ISampleService>(ServiceLifetime.Singleton);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ThrowException_When_CreateClientIsNotCalled()
        {
            // Given
            // When
            Action callVerify = () => SUT.VerifyRegisteredLifeTimeOfService<ISampleService>(ServiceLifetime.Scoped);

            // Then
            callVerify.Should().Throw<InvalidOperationException>();
        }
    }
}