using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wd3w.AspNetCore.EasyTesting.EntityFrameworkCore;
using Wd3w.AspNetCore.EasyTesting.Hestify;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Entities;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.InMemoryDbContext
{
    public class InMemoryDbContextHelperTest : EasyTestingTestBase
    {
        [Fact]
        public async Task ReplaceInMemoryDbContextTest()
        {
            // Given
            SUT.ReplaceInMemoryDbContext<SampleDb>()
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
        
        [Fact]
        public async Task SampleApiE2ETest()
        {
            // Given
            SUT.ReplaceInMemoryDbContext<SampleDb>()
                .SetupFixture<SampleDb>(async db =>
                {
                    await db.SampleDataEntities.AddRangeAsync(new[]
                    {
                        new SampleDataEntity {Data = "Hi"},
                        new SampleDataEntity {Data = "Hi"},
                        new SampleDataEntity {Data = "Hi"},
                        new SampleDataEntity {Data = "Hi"}
                    });

                    await db.SaveChangesAsync();
                });
            
            // When
            var message = await SUT.Resource("api/sample/sample-data-from-db").GetAsync();
            
            // Then
            await message.ShouldBeOk<IEnumerable<SampleDataEntity>>(entities =>
            {
                entities.Should().HaveCount(4);
            });
        }

    }
}