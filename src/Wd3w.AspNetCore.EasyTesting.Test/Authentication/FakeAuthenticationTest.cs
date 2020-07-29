using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Hestify;
using Microsoft.AspNetCore.Authentication;
using Wd3w.AspNetCore.EasyTesting.Authentication;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.Authentication
{
    public class FakeAuthenticationTest : EasyTestingTestBase
    {
        [Fact]
        public async Task Test()
        {
            var httpClient = SUT
                .NoUserAuthentication("Bearer")
                .CreateClient();
            
            var message = await httpClient.Resource("api/sample/secure").GetAsync();
            
            message.ShouldBe(HttpStatusCode.Unauthorized);

            SUT.FakeAuthentication("Bearer", AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, "test@test.com"),
                }, "Bearer")), "Bearer")));
            var message2 = await httpClient.Resource("api/sample/secure").GetAsync();
             message2.ShouldBe(HttpStatusCode.NoContent);
        }
    }
}