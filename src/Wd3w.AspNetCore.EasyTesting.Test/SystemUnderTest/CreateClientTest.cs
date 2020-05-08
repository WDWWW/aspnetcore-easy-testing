using System.Threading.Tasks;
using FluentAssertions;
using Hestify;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class CreateClientTest : SystemUnderTestBase
    {
        [Fact]
        public async Task CreateClient_Should_ReturnHttpClientAndWork()
        {
            // Given
            // WHen
            var httpClient = SUT.CreateClient();
            var response = await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            var sample = await response.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Original Sample Data");
        }
    }
}