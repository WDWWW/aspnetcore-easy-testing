using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Wd3w.AspNetCore.EasyTesting.Test.SystemUnderTest
{
    public class ReplaceLoggerFactoryTest : EasyTestingTestBase
    {
        private readonly ITestOutputHelper _helper;

        public ReplaceLoggerFactoryTest(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        [Fact]
        public async Task Should_CallWriteLine_When_ReplaceLoggerFactory()
        {
            // Given
            var mock = new Mock<ITestOutputHelper>();
            SUT.ReplaceLoggerFactory(builder => builder.AddXUnit(mock.Object).AddXUnit(_helper));
            
            // When
            var client = SUT.CreateClient();
            await client.GetAsync("api/sample/configuration");
            
            // Then
            mock.Verify(helper => helper.WriteLine(It.IsAny<string>()), Times.AtLeast(1));
        }
    }
}