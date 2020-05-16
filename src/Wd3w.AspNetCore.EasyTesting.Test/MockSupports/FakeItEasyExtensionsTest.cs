using FakeItEasy;
using FluentAssertions;
using Wd3w.AspNetCore.EasyTesting.FakeItEasy;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.MockSupports
{
    public class FakeItEasyExtensionsTest : EasyTestingTestBase
    {
        [Fact]
        public void DummyService_Should_ReturnEmpty_When_ServiceIsReplacedWithDummy()
        {
            // Given
            // When
            SUT.ReplaceDummyService<ISampleService>()
                .CreateClient();

            // THen
            SUT.UsingService<ISampleService>(service => service.GetSampleDate().Should().BeEmpty());
        }

        [Fact]
        public void FakeService_Should_CanBeMocked_When_ServiceIsReplacedFakeObject()
        {
            // Given
            // When
            SUT.FakeService<ISampleService>(service => A
                    .CallTo(() => service.GetSampleDate())
                    .Returns("MockedData"))
                .CreateClient();

            // Then
            SUT.UsingService<ISampleService>(service => service.GetSampleDate().Should().Be("MockedData"));
        }

        [Fact]
        public void GetFakeService_Should_ReturnSameObjectWithRegisteredObject_When_ServiceIsFaked()
        {
            // Given
            var service = SUT.FakeService<ISampleService>();

            // When
            SUT.CreateClient();

            // Then
            var fakeService = SUT.GetFakeService<ISampleService>();
            fakeService.Should().BeSameAs(service);
        }

        [Fact]
        public void UseFakeService_Should_ProvideSameObjectWithRegisteredObject_When_ServiceIsFaked()
        {
            // Given
            var service = SUT.FakeService<ISampleService>();

            // When
            SUT.CreateClient();

            // Then
            SUT.UseFakeService<ISampleService>(fake => fake.Should().BeSameAs(service));
        }
    }
}