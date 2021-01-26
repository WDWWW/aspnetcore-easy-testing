using Microsoft.AspNetCore.Hosting;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class DisableStartupFiltersTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_RemoveIStartFilterRegistration_When_CallDisableStartupFilters()
        {
            // Given
            SUT.DisableStartupFilters();

            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration<IStartupFilter>();
        }
    }
}