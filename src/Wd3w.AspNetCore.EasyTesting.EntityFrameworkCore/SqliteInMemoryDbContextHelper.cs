using Microsoft.EntityFrameworkCore;

namespace Wd3w.AspNetCore.EasyTesting.EntityFrameworkCore
{
    public static class SqliteInMemoryDbContextHelper
    {
        private static DbContextOptions<TDbContext> CreateInMemoryDbContextOptions<TDbContext>()
            where TDbContext : DbContext
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
        }

        private static DbContextOptions CreateInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder()
                .UseSqlite("DataSource=:memory:")
                .Options;
        }

        public static SystemUnderTest ReplaceSqliteInMemoryDbContext<TDbContext>(
            this SystemUnderTest sut)
            where TDbContext : DbContext
        {
            return sut.ReplaceService(CreateInMemoryDbContextOptions())
                .ReplaceService(CreateInMemoryDbContextOptions<TDbContext>())
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