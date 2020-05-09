using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wd3w.AspNetCore.EasyTesting.EntityFrameworkCore;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Entities;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.InMemoryDbContext
{
    public class SqliteInMemoryDbContextHelperTest : EasyTestingTestBase
    {
        [Fact]
        public async Task ReplaceSqliteInMemoryDbContextTest()
        {
            // Given
            SUT.ReplaceSqliteInMemoryDbContext<SampleDb>()
                .SetupFixture<SampleDb>(async db =>
                {
                    await db.SampleDataEntities.AddAsync(new SampleDataEntity
                    {
                        Data = "Sample Data"
                    });
                    await db.SaveChangesAsync();
                });

            // When
            SUT.CreateClient();

            // Then
            await SUT.UsingServiceAsync<SampleDb>(async db => (await db.SampleDataEntities.CountAsync()).Should().Be(1));
        }
    }
}