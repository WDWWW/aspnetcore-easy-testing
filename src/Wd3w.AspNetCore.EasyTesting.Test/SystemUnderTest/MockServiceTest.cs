using System.Threading.Tasks;
using FluentAssertions;
using Hestify;
using Moq;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class MockServiceTest : SystemUnderTestBase
    {
        [Fact]
        public async Task MockService_Should_ReplaceWithMockObjectOfServiceType()
        {
            // Given
            var httpClient = SUT.MockService<ISampleService>(out var mock)
                .CreateClient();
            mock.Setup(service => service.GetSampleDate())
                .Returns("Mock Return!");

            // When
            var message = await httpClient.Resource("api/sample/data").GetAsync();

            // Then
            var sample = await message.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Mock Return!");
            mock.Verify(service => service.GetSampleDate(), Times.Once);
        }
    }
}