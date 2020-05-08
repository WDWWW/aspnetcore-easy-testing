using System.Threading.Tasks;
using FluentAssertions;
using Hestify;
using Wd3w.AspNetCore.EasyTesting.SampleApi;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.EasyIntegrationTester
{
    public class EasyIntegrationTesterCreateClientTest : IClassFixture<TestApiFactory>
    {
        private readonly EasyIntegrationTester<Startup> _easyTester;

        public EasyIntegrationTesterCreateClientTest(TestApiFactory factory)
        {
            _easyTester = factory.Easy();
        }

        [Fact]
        public async Task CreateClient_Should_ReturnHttpClientAndWork()
        {
            // Given
            // WHen
            var httpClient = _easyTester.CreateClient();
            var response = await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            var sample = await response.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Original Sample Data");
        }
    }
}