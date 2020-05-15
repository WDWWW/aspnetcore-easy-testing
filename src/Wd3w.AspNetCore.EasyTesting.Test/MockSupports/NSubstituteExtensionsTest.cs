using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Wd3w.AspNetCore.EasyTesting.Hestify;
using Wd3w.AspNetCore.EasyTesting.NSubstitute;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Models;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.MockSupports
{
    public class NSubstituteExtensionsTest : EasyTestingTestBase
    {
        [Fact]
        public void ReplaceWithNSubstitute_Should_RegisterOnInternalService_When_ServiceIsReplacedWithSubstitute()
        {
            // Given
            SUT.ReplaceWithSubstitute<ISampleService>(out var mockedService);

            // When
            SUT.CreateClient();

            // Then
            SUT.UsingService<ISampleService>(service => service.Should().BeSameAs(mockedService));
        }

        [Fact]
        public async Task ReplaceWithNSubstitute_Should_ReturnMockedDataFromSubstitute_When_ServiceIsReplacedWithSubstitute()
        {
            // Given
            SUT.ReplaceWithSubstitute<ISampleService>(service => service.GetSampleDate().Returns("Mocked Data"));

            // When
            var message = await SUT.Resource("api/sample/data").GetAsync();

            // Then
            await message.ShouldBeOk<SampleDataResponse>(res => res.Data.Should().Be("Mocked Data"));
        }

        [Fact]
        public async Task ReplaceWithNSubstitute_Should_CheckCallGetSampleData_When_ServiceIsReplacedWithSubstitute()
        {
            // Given
            SUT.ReplaceWithSubstitute<ISampleService>(service => service.GetSampleDate().Returns("Mocked Data"));

            // When
            await SUT.Resource("api/sample/data").GetAsync();

            // Then
            SUT.GetSubstitute<ISampleService>().Received().GetSampleDate();
        }

        [Fact]
        public async Task
            ReplaceWithNSubstitute_Should_ReturnSameMockedObject_When_ReplaceWithNSubstituteIsCalledMultipleTimes()
        {
            // Given
            // When
            SUT.ReplaceWithSubstitute<ISampleService>(out var s1)
                .ReplaceWithSubstitute<ISampleService>(out var s2);

            // Then
            s1.Should().BeSameAs(s2);
        }

        [Fact]
        public void GetSubstitute_Should_ReturnSameAsRegisteredMockObject_When_TheServiceIsAlreadyMocked()
        {
            // Given
            SUT.ReplaceWithSubstitute<ISampleService>(out var service);

            // When
            var gettingService = SUT.GetSubstitute<ISampleService>();

            // Then
            service.Should().BeSameAs(gettingService);
        }

        [Fact]
        public void
            UseSubstitute_Should_SameObjectBetweenPreRegisteredObjectAndProvidedObject_When_TheServiceIsAlreadyMocked()
        {
            // Given
            // When
            SUT.ReplaceWithSubstitute<ISampleService>(out var service);

            // Then
            SUT.UseSubstitute<ISampleService>(mock => service.Should().BeSameAs(mock));
        }
    }
}