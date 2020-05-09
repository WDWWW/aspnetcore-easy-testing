using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class UsingServiceAsyncTest : EasyTestingTestBase
    {
        [Fact]
        public async Task Should_CallOnceServiceMethod_When_ServiceIsUsedInUsingServiceAsync()
        {
            // Given
            SUT.MockService<ISampleService>(out var mock)
                .CreateClient();

            // When
            await SUT.UsingServiceAsync<ISampleService>(service =>
            {
                service.GetSampleDate();
                return Task.CompletedTask;
            });

            // Then
            mock.Verify(service => service.GetSampleDate(), Times.Once);
        }

        [Fact]
        public void Should_ThrowEcxeption_When_CreateClientIsNotCalledYet()
        {
            // Given
            // When
            Action callUsingService = () => SUT.UsingServiceAsync<ISampleService>(service => Task.CompletedTask).Wait();

            // Then
            callUsingService.Should().Throw<InvalidOperationException>();
        }
    }
}