using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;

namespace Wd3w.AspNetCore.EasyTesting
{
    public static class HttpResponseMessageAssertionHelper
    {
        public static void ShouldBeOk(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.OK);
        }

        public static async Task<TResponse> ShouldBeOk<TResponse>(this HttpResponseMessage message)
        {
            return await message.ShouldBeCodeWithBody<TResponse>(HttpStatusCode.OK);
        }

        public static void ShouldBeNoContent(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.NoContent);
        }

        public static void ShouldBeAccepted(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.Accepted);
        }

        public static void ShouldBeBadRequest(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.BadRequest);
        }

        public static void ShouldBeForbidden(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.Forbidden);
        }

        public static void ShouldBe(this HttpResponseMessage message, HttpStatusCode code)
        {
            message.StatusCode.Should().Be(code);
        }

        public static async Task<TResponse> ShouldBeCodeWithBody<TResponse>(this HttpResponseMessage message, HttpStatusCode code)
        {
            message.ShouldBe(code);
            return await message.JsonBody<TResponse>();
        }

        public static async Task<TResponse> JsonBody<TResponse>(this HttpResponseMessage message)
        {
            var body = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(body);
        }
    }
}