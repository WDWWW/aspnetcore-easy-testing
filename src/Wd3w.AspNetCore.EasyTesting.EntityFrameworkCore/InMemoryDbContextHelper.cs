using System;
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

        /// <summary>
        ///     Replace DbContextOptions and DbContextOptions<TDbContext> to InMemoryDbContextOptions and InMemoryDbContextOptions<TDbContext>
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="databaseName"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static SystemUnderTest ReplaceInMemoryDbContext<TDbContext>(
            this SystemUnderTest sut, string databaseName = default)
            where TDbContext : DbContext
        {
            databaseName ??= Guid.NewGuid().ToString();
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