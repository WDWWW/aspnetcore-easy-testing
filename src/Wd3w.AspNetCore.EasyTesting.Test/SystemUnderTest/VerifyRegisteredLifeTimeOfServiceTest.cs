using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class VerifyRegisteredLifeTimeOfServiceTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_ReturnTrue_When_SampleServiceIsRegisteredAsScoped()
        {
            // Given
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyRegisteredLifeTimeOfService<ISampleService>(ServiceLifetime.Scoped);
        }

        [Fact]
        public void Should_ReturnTrue_When_SampleServiceIsRegistredSingleTonAndReplaceServiceWithSingleton()
        {
            // Given
            SUT.ReplaceService<ISampleService>(new SampleService());

            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyRegisteredLifeTimeOfService<ISampleService>(ServiceLifetime.Singleton);
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