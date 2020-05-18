using System.Threading.Tasks;
using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.EntityFrameworkCore;
using Wd3w.AspNetCore.EasyTesting.Hestify;
using Wd3w.AspNetCore.EasyTesting.Moq;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Entities;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test
{
    public class SampleTest : EasyTestingTestBase
    {
        [Fact]
        public async Task EasyTesting_Make_IntegrationTest_Easy()
        {
            // Given
            SUT.ReplaceInMemoryDbContext<SampleDb>()
                .SetupFixture<SampleDb>(async db =>
                {
                    db.SampleDataEntities.Add(new SampleDataEntity());
                    await db.SaveChangesAsync();
                })
                .MockService<ISampleService>(mock => mock
                    .Setup(s => s.GetSampleDate())
                    .Returns("MockedData"));

            // When
            await SUT.Resource("api/get/sample").GetAsync();

            // Then
            SUT.UsingService<ISampleService>(service => service.GetSampleDate().Should().Be("MockedData"));
            SUT.VerifyCallOnce<ISampleService>(service => service.GetSampleDate());
        }
    }
}