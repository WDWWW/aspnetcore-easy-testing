using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class RemoveAllByTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_RemoveAllRegistration_When_ConditionIsDetermineSampleService()
        {
            // Given
            SUT.RemoveAllBy(descriptor => descriptor.ServiceType == typeof(ISampleService));

            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration<ISampleService>();
        } 
    }
}