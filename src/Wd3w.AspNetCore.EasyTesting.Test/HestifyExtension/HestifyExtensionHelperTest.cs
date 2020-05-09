using System.Threading.Tasks;
using Wd3w.AspNetCore.EasyTesting.Hestify;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.HestifyExtension
{
    public class HestifyExtensionHelperTest : EasyTestingTestBase
    {
        [Fact]
        public async Task Test()
        {
            // Given
            // When
            var message = await SUT.Resource("api/sample/data").GetAsync();

            // Then
            message.ShouldBeOk();
        }
    }
}