using System.Net;
using Hestify;

namespace Wd3w.AspNetCore.EasyTesting.Test.Helper
{
    public static class HestifyClientHelper
    {
        public static HestifyClient WithBearerToken(this HestifyClient client, string token)
        {
            return client.WithHeader(HttpRequestHeader.Authorization, $"Bearer ${token}");
        }

        public static HestifyClient WithFakeBearerToken(this HestifyClient client)
        {
            return client.WithBearerToken("01234567890123456789");
        }
    }
}