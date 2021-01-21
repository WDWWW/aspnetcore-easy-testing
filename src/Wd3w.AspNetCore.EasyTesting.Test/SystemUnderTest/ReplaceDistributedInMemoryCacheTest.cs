using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class ReplaceDistributedInMemoryCacheTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_RegisterRedisCacheAsDefault()
        {
            // Given
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyRegisteredImplementationTypeOfService<IDistributedCache, RedisCache>();
        }

        [Fact]
        public void
            Should_ReplaceImplementationTypeOfCacheToMemoryDistributedCache_When_ReplaceToMemoryCacheBeforeCreateClient()
        {
            // Given
            SUT.ReplaceDistributedInMemoryCache();
            
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyRegisteredImplementationTypeOfService<IDistributedCache, MemoryDistributedCache>();
        }
    }
}