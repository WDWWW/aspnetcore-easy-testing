using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Wd3w.AspNetCore.EasyTesting.EntityFrameworkCore
{
    public static class SqliteInMemoryDbContextHelper
    {
        private static DbContextOptions<TDbContext> CreateInMemoryDbContextOptions<TDbContext>()
            where TDbContext : DbContext
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite(CreateInMemoryConnection())
                .Options;
        }

        private static DbContextOptions CreateInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder()
                .UseSqlite(CreateInMemoryConnection())
                .Options;
        }

        private static SqliteConnection CreateInMemoryConnection()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        /// <summary>
        ///     Replace DbContextOptions for TDbContext to InMemory SqliteDbContextOptions
        /// </summary>
        /// <param name="sut">System under test</param>
        /// <typeparam name="TDbContext">DbContext type</typeparam>
        /// <returns></returns>
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