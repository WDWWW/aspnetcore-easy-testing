using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hestify;
using Moq;
using Wd3w.AspNetCore.EasyTesting.Moq;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class MockServiceTest : EasyTestingTestBase
    {
        [Fact]
        public async Task Should_ReplaceWithMockObjectOfServiceType()
        {
            // Given
            var httpClient = SUT.MockService<ISampleService>(out var mock)
                .CreateClient();

            mock.Setup(service => service.GetSampleDate())
                .Returns("Mock Return!");

            // When
            var message = await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            var sample = await message.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Mock Return!");
            mock.Verify(service => service.GetSampleDate(), Times.Once);
        }

        [Fact]
        public async Task Should_ReplaceWithMockObjectOfServiceType_When_ProvidedByAction()
        {
            // Given
            var httpClient = SUT.MockService<ISampleService>(_ => _
                    .Setup(service => service.GetSampleDate())
                    .Returns("Action mocked!"))
                .CreateClient();

            // When
            var message = await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            var sample = await message.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Action mocked!");
        }

        [Fact]
        public void Should_ThrowException_When_TryToCallMockServiceAfterCreateClient()
        {
            // Given
            SUT.CreateClient();

            // When
            Action callMockService = () => SUT.MockService<ISampleService>(out _);

            // Then
            callMockService.Should().Throw<InvalidOperationException>();
        }
    }
}