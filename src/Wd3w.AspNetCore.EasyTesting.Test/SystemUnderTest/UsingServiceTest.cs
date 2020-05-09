using System;
using FluentAssertions;
using Moq;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class UsingServiceTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_CallOnceServiceMethod_When_ServiceIsUsedInUsingServiceAsync()
        {
            // Given
            SUT.MockService<ISampleService>(out var mock)
                .CreateClient();

            // When
            SUT.UsingService<ISampleService>(service => service.GetSampleDate());

            // Then
            mock.Verify(service => service.GetSampleDate(), Times.Once);
        }

        [Fact]
        public void Should_ThrowException_When_CreateClientIsNotCalledYet()
        {
            // Given
            // When
            Action callUsingService = () => SUT.UsingService<ISampleService>(service => { });

            // Then
            callUsingService.Should().Throw<InvalidOperationException>();
        }
    }
}