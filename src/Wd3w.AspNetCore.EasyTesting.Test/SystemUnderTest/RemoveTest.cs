using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class RemoveTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_DontBeRegisteredSampleService_When_RemoveISampleServiceTypeAsGeneric()
        {
            // Given
            SUT.Remove<ISampleService, SampleService>();

            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration<ISampleService>();
        }

        [Fact]
        public void Should_DontBeRegisteredSampleService_When_RemoveISampleServiceTypeAsParameter()
        {
            // Given
            SUT.Remove(typeof(ISampleService), typeof(SampleService));

            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration<ISampleService>();
        }
    }
}