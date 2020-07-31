using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Hestify;
using Wd3w.AspNetCore.EasyTesting.Authentication;
using Wd3w.AspNetCore.EasyTesting.Test.Common;
using Wd3w.AspNetCore.EasyTesting.Test.Helper;
using Xunit;

namespace Wd3w.AspNetCore.EasyTesting.Test.Authentication
{
    public class NoUserAuthenticationTest : EasyTestingTestBase
    {
        [Fact]
        public async Task PassTest()
        {
            var httpClient = SUT.CreateClient();
            
            var message = await CallAuthorizedApiWithAccessTokenAsync(httpClient);
            
            message.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task NoUserAuthenticationDefaultSchemeTest()
        { 
            var httpClient = SUT
                .NoUserAuthentication()
                .CreateClient();
            
            var message = await CallAuthorizedApiWithAccessTokenAsync(httpClient);
            
            message.ShouldBe(HttpStatusCode.Unauthorized);
        }

        private static Task<HttpResponseMessage> CallAuthorizedApiWithAccessTokenAsync(HttpClient httpClient)
        {
            return httpClient
                .Resource("api/sample/secure")
                .WithFakeBearerToken()
                .GetAsync();
        }

        [Fact]
        public async Task NoUserAuthenticationSpecificSchemeTest()
        {
            var httpClient = SUT
                .NoUserAuthentication("Bearer")
                .CreateClient();

            var message = await CallAuthorizedApiWithAccessTokenAsync(httpClient);

            message.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void NoUserAuthenticationWithWrongAuthorizationSchemeTest()
        {
            Func<HttpClient> createClient = SUT.NoUserAuthentication("WrongScheme").CreateClient;

            createClient.Should().Throw<Exception>();
        }
    }
}