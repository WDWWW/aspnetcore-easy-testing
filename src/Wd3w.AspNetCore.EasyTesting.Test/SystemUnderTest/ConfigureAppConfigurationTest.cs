using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class ConfigureAppConfigurationTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_CallBackIsCalled_When_CreateClient()
        {
            var called = false;
            SUT.ConfigureAppConfiguration(builder => called = true)
                .CreateClient();

            called.Should().BeTrue();
        }
    }
}