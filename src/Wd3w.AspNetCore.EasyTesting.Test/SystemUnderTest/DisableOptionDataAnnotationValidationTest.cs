using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Wd3w.AspNetCore.EasyTesting.SampleApi;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class DisableOptionDataAnnotationValidationTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_BeWorkingNormally_When_AllConfigurationIsExactlyFine()
        {
            // Given
            // When
            SUT.CreateClient();
            
            // Then
            SUT.UsingService<IOptionsSnapshot<SampleOption>>(snapshot => snapshot.Value.NeedValue.Should().Be("Sample"));
        }

        [Fact] 
        public void Should_ThrowOptionsValidationException_When_ConfigurationIsInvalid()
        {
            // Given
            SUT.OverrideAppConfiguration(new {NeedValue = ""});
            
            // When
            Action startup = () =>  SUT.CreateClient();

            // Then
            startup.Should().Throw<OptionsValidationException>();
        }

        [Fact]
        public void Should_NotThingToBeThrown_When_DisableOptionDataAnnotationValidationAlthoughConfigurationIsInvalid()
        {
            // Given
            SUT.OverrideAppConfiguration(new {NeedValue = ""})
                .DisableOptionDataAnnotationValidation<SampleOption>();

            // When
            Action startup = () => SUT.CreateClient();

            // Then
            startup.Should().NotThrow();
        }

        [Fact]
        public void Should_NothingToBeThrown_When_DisableAllOptionValidationsAlthoughConfigurationIsInvalid()
        {
            // Given
            SUT.OverrideAppConfiguration(new {NeedValue = ""})
                .DisableOptionValidations<SampleOption>();

            // When
            Action startup = () => SUT.CreateClient();

            // Then
            startup.Should().NotThrow();
        }
    }
}