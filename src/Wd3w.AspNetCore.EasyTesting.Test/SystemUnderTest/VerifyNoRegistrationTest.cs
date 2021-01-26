using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class VerifyNoRegistrationTest : EasyTestingTestBase
    {
        [Fact]
        public void Should_Verify_With_OneServiceTypeGenericParameter()
        {
            // Given
            // When
            SUT.CreateClient();
            
            // Then
            SUT.VerifyNoRegistration<INeverRegisteredServiceType>();
        }

        [Fact]
        public void Should_Verify_With_OneServiceTypeInstanceParameter()
        {
            // Given
            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration(typeof(INeverRegisteredServiceType));
        }

        [Fact]
        public void Should_Verify_With_TwoTypeParameters()
        {
            // Given
            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration<INeverRegisteredServiceType, NeverRegisteredServiceType>();
        }

        [Fact]
        public void Should_Verify_With_TwoParameter()
        {
            // Given
            // When
            SUT.CreateClient();

            // Then
            SUT.VerifyNoRegistration(typeof(INeverRegisteredServiceType), typeof(NeverRegisteredServiceType));
        }
    }
}