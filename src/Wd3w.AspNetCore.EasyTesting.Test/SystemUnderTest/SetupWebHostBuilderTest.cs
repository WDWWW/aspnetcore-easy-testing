using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class SetupWebHostBuilderTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_CalledSetupWebHostBuilderAction_When_CreateClientIsCalled()
        {
            // Given
            var called = false;
            SUT.SetupWebHostBuilder(builder => called = true);

            // When
            SUT.CreateClient();

            // Then
            called.Should().BeTrue();
        }
    }
}