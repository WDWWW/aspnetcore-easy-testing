using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class UseSettingTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_SetSettingValue_When_KeyAndValueIsProvided()
        {
            SUT.UseSetting("config-key", "value");

            string configSettingValue = default;
            SUT
                .SetupWebHostBuilder(builder => configSettingValue = builder.GetSetting("config-key"))
                .CreateClient();

            configSettingValue.Should().Be("value");
        }
    }
}