using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hestify;
using Microsoft.Extensions.DependencyInjection;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class ReplaceServiceTest : SystemUnderTestBase
    {
        public class FakeSampleService : ISampleService
        {
            public string Message { get; set; } = "Fake!";

            public string GetSampleDate()
            {
                return Message;
            }
        }

        public class GuidSampleService : ISampleService
        {
            public string Message { get; set; } = Guid.NewGuid().ToString();

            public string GetSampleDate()
            {
                return Message;
            }
        }

        [Fact]
        public async Task ReplaceService_Should_ReplaceService()
        {
            var httpClient = SUT
                .ReplaceService<ISampleService, FakeSampleService>()
                .CreateClient();

            var response = await httpClient.Resource("api/sample/data").GetAsync();

            var sample = await response.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Fake!");
        }

        [Fact]
        public async Task ReplaceService_Should_RegisterServiceAsSingleton_With_ScopeIsSingleton()
        {
            var httpClient = SUT
                .ReplaceService<ISampleService, GuidSampleService>(ServiceLifetime.Singleton)
                .CreateClient();

            var response1 = await httpClient.Resource("api/sample/data").GetAsync();
            var response2 = await httpClient.Resource("api/sample/data").GetAsync();

            var sample1 = await response1.ReadJsonBodyAsync<SampleDataResponse>();
            var sample2 = await response2.ReadJsonBodyAsync<SampleDataResponse>();
            sample1.Data.Should().Be(sample2.Data);
        }

        [Fact]
        public async Task ReplaceService_Should_RegisterServiceLifeTimeIsSameWithOriginal_With_ScopeIsNotProvided()
        {
            var httpClient = SUT
                .ReplaceService<ISampleService, GuidSampleService>()
                .CreateClient();

            var response1 = await httpClient.Resource("api/sample/data").GetAsync();
            var response2 = await httpClient.Resource("api/sample/data").GetAsync();

            var sample1 = await response1.ReadJsonBodyAsync<SampleDataResponse>();
            var sample2 = await response2.ReadJsonBodyAsync<SampleDataResponse>();
            sample1.Data.Should().NotBe(sample2.Data);
        }

        [Fact]
        public async Task ReplaceService_Should_ReplaceService_When_ImplementationObject()
        {
            var serviceObject = new FakeSampleService
            {
                Message = "Fake More!"
            };
            var httpClient = SUT
                .ReplaceService<ISampleService>(serviceObject)
                .CreateClient();

            var response = await httpClient.Resource("api/sample/data").GetAsync();

            var sample = await response.ShouldBeOk<SampleDataResponse>();
            sample.Data.Should().Be("Fake More!");
        }
    }
}