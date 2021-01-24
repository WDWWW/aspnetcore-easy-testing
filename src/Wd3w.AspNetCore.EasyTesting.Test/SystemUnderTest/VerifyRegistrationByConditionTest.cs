using System;
using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;
using Xunit.Sdk;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class VerifyRegistrationByConditionTest : EasyTestingTestBase
    {
        [Fact]
        public void SuccessTest()
        {
            // Given
            var fakeSampleService = new FakeSampleService();
            SUT.ReplaceService<ISampleService>(fakeSampleService);
            
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyRegistrationByCondition(descriptor => descriptor.ServiceType == typeof(ISampleService) &&
                descriptor.ImplementationInstance == fakeSampleService);
        }
        
        [Fact]
        public void FailTest()
        {
            // Given
            SUT.ReplaceService<ISampleService>(new FakeSampleService());
            
            // When
            SUT.CreateClient();
            
            // Then
            Action action = () => SUT.VerifyRegistrationByCondition(descriptor => descriptor.ServiceType == typeof(ISampleService) && descriptor.ImplementationInstance == null);
            action.Should().Throw<XunitException>();
        }
    }
}