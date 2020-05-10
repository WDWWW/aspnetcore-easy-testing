using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class ConfigureServicesTest : EasyTestingTestBase
    {
        public class TestService{}

        [Fact]
        public void Should_BeRegistered_When_AdditionalServiceIsAddedByConfigureService()
        {
            // Given
            SUT.ConfigureServices(services => services.AddSingleton(new TestService()));

            // When
            SUT.CreateClient();

            // Then
            SUT.UsingService<TestService>(service => service.Should().NotBeNull());
        }
    }
}