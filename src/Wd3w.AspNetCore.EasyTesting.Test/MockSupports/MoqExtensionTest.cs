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

namespace Wd3w.AspNetCore.EasyTesting.Test.MockSupports
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

        [Fact]
        public void Should_ReturnMockObject_When_RegisteredMockObject()
        {
            // Given
            SUT.MockService<ISampleService>(out _)
                .CreateClient();

            // When
            var mock = SUT.GetServiceMock<ISampleService>();

            // Then
            mock.Should().NotBeNull();
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_TheServiceIsNotReplacedWithMock()
        {
            // Given
            SUT.CreateClient();

            // When
            Action action = () => SUT.GetServiceMock<ISampleService>();

            // Then
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task Should_CanVerifyCalledWithVerifyCallWithTimesOnce_When_TheApiIsCalled()
        {
            // Given
            var httpClient = SUT.MockService<ISampleService>(_ => _
                    .Setup(service => service.GetSampleDate())
                    .Returns("Mock Return!"))
                .CreateClient();

            // When
            await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            SUT.VerifyCall<ISampleService>(service => service.GetSampleDate(), Times.Once());
        }

        [Fact]
        public async Task Should_VerifyAtLeastOnce_When_TheTimesParameterIsNotProvided()
        {
            // Given
            var httpClient = SUT.MockService<ISampleService>(_ => _
                    .Setup(service => service.GetSampleDate())
                    .Returns("Mock Return!"))
                .CreateClient();

            // When
            await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            SUT.VerifyCall<ISampleService>(service => service.GetSampleDate());
        }

        [Fact]
        public async Task Should_VerifyCallOnce_When_TheApiIsCalledOneTime()
        {
            // Given
            var httpClient = SUT.MockService<ISampleService>(_ => _
                    .Setup(service => service.GetSampleDate())
                    .Returns("Mock Return!"))
                .CreateClient();

            // When
            await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            SUT.VerifyCallOnce<ISampleService>(service => service.GetSampleDate());
        }

        [Fact]
        public void GetServiceMock_Should_GettingMockObjectIsSameAsPreviousMockObject_When_TheServiceIsAlreadyMocked()
        {
            // Given
            var mockService = SUT.MockService<ISampleService>();

            // When
            var gettingService = SUT.GetServiceMock<ISampleService>();

            // Then
            mockService.Should().BeSameAs(gettingService);
        }

        [Fact]
        public void MockService_Should_ReturnSameMockObject_When_MockServiceIsCalledMultipleTimes()
        {
            // Given
            // When
            var m1 = SUT.MockService<ISampleService>();
            var m2 = SUT.MockService<ISampleService>();

            // Then
            m1.Should().BeSameAs(m2);
        }

        [Fact]
        public void UseServiceMock_Should_ReturnSameMockObject_When_TheServiceIsReplacedAlready()
        {
            // Given
            // When
            var mockService = SUT.MockService<ISampleService>();

            // Then
            SUT.UseServiceMock<ISampleService>(mock => mock.Should().BeSameAs(mockService));
        }
    }
}