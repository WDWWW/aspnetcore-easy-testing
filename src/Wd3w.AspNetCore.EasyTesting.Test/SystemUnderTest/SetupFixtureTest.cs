using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class SetupFixtureTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_CallOnceServiceMethod_When_CreateClient()
        {
            // Given
            SUT.MockService<ISampleService>(out var mock)
                .SetupFixture<ISampleService>(service =>
                {
                    // Something do buildup fixture
                    service.GetSampleDate();
                    return Task.CompletedTask;
                });

            // When
            SUT.CreateClient();

            // Then
            mock.Verify(service => service.GetSampleDate(), Times.Once);
        }

        [Fact]
        public void Should_CallOnceServiceMethod_When_CreateDefaultClient()
        {
            // Given
            SUT.MockService<ISampleService>(out var mock)
                .SetupFixture<ISampleService>(service =>
                {
                    // Something do buildup fixture
                    service.GetSampleDate();
                    return Task.CompletedTask;
                });

            // When
            SUT.CreateDefaultClient();

            // Then
            mock.Verify(service => service.GetSampleDate(), Times.Once);
        }

        [Fact]
        public void Should_ThrowException_When_TryToCallSetupFixtureAfterCreateClient()
        {
            // Given
            SUT.CreateClient();

            // When
            Action callSetupFixture = () => SUT.SetupFixture<ISampleService>(service => Task.CompletedTask);

            // Then
            callSetupFixture.Should().Throw<InvalidOperationException>();
        }
    }
}