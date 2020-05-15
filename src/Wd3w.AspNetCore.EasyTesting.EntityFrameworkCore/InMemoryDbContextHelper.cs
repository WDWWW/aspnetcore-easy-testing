using Microsoft.EntityFrameworkCore;

namespace Wd3w.AspNetCore.EasyTesting.EntityFrameworkCore
{
    public static class InMemoryDbContextHelper
    {
        private static DbContextOptions<TDbContext> CreateInMemoryDbContextOptions<TDbContext>(string databaseName)
            where TDbContext : DbContext
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        private static DbContextOptions CreateInMemoryDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        public static SystemUnderTest ReplaceInMemoryDbContext<TDbContext>(
            this SystemUnderTest sut, string databaseName = "in-memory")
            where TDbContext : DbContext
        {
            return sut.ReplaceService(CreateInMemoryDbContextOptions(databaseName))
                .ReplaceService(CreateInMemoryDbContextOptions<TDbContext>(databaseName))
                .SetupFixture<TDbContext>(async context =>
                {
                    var database = context.Database;
                    await database.OpenConnectionAsync();
                    await database.EnsureDeletedAsync();
                    await database.EnsureCreatedAsync();
                });
        }
    }
}