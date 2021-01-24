using FluentAssertions;
using Microsoft.Extensions.Options;
using Wd3w.AspNetCore.EasyTesting.SampleApi;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class ReplaceConfigureOptionsTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_ReplaceOptionConfigurer_When_CallReplaceConfigureOptionsWithCustomCallBack()
        {
            // Given
            var anotherValue = "another value";
            SUT.ReplaceConfigureOptions<SampleOption>(option => option.NeedValue = anotherValue);
            
            // When
            SUT.CreateClient();
            
            // Then
            SUT.UsingService<IOptionsSnapshot<SampleOption>>(snapshot => snapshot.Value.NeedValue.Should().Be(anotherValue));
        }
    }
}