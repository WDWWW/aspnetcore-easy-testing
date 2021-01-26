using System;
using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;
using Xunit.Sdk;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class VerifyNoRegistrationByConditionTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_VerifyToSuccess_When_ServiceTypeNeverRegistered()
        {
            // Given
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyNoRegistrationByCondition(descriptor => descriptor.ServiceType == typeof(INeverRegisteredServiceType));
        }

        [Fact]
        public void Should_VerifyToFail_When_ServiceTypeNeverRegistered()
        {
            // Given
            // When
            SUT.CreateClient();
            Action verify = () => SUT.VerifyNoRegistrationByCondition(descriptor => descriptor.ServiceType == typeof(ISampleService));

            // Then
            verify.Should().Throw<XunitException>();
        }
    }
}