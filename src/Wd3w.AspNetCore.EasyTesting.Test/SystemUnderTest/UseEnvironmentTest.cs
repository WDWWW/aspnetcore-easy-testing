using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Wd3w.AspNetCore.EasyTesting.Hestify;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class UseEnvironmentTest : EasyTestingTestBase
    {

        [Fact]
        public async Task UseDevelopmentEnvironmentTest()
        {
            // Given
            SUT.UseDevelopmentEnvironment();
            
            // When
            var message = await SUT.Resource("api/sample/environment").GetAsync();
            
            // THen
            var environment = await message.Content.ReadAsStringAsync();
            environment.Should().Be(Environments.Development);
        }
        
        [Fact]
        public async Task UseStagingEnvironmentTest()
        {
            // Given
            SUT.UseStagingEnvironment();
            
            // When
            var message = await SUT.Resource("api/sample/environment").GetAsync();
            
            // THen
            var environment = await message.Content.ReadAsStringAsync();
            environment.Should().Be(Environments.Staging);
        }
        
        [Fact]
        public async Task UseProductionEnvironmentTest()
        {
            // Given
            SUT.UseProductionEnvironment();
            
            // When
            var message = await SUT.Resource("api/sample/environment").GetAsync();
            
            // Then
            var environment = await message.Content.ReadAsStringAsync();
            environment.Should().Be(Environments.Production);
        }

        [Fact]
        public async Task UseSpecificEnvironmentTest()
        {
            // Given
            SUT.UseEnvironment("Testing");
            
            // When
            var message = await SUT.Resource("api/sample/environment").GetAsync();
            
            // Then
            var environment = await message.Content.ReadAsStringAsync();
            environment.Should().Be("Testing");
        }
    }
}