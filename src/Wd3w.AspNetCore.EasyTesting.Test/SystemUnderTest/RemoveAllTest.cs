using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class RemoveAllTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_DontBeRegisteredSampleService_When_RemoveISampleServiceTypeAsGeneric()
        {
            // Given
            SUT.RemoveAll<ISampleService>();
            
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyNoRegistration<ISampleService>();
        }

        [Fact]
        public void Should_DontBeRegisteredSampleService_When_RemoveISampleServiceTypeAsParameter()
        {
            // Given
            SUT.RemoveAll(typeof(ISampleService));

            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration<ISampleService>();
        }
    }
}