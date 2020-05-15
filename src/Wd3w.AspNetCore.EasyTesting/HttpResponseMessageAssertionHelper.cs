using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;

namespace Wd3w.AspNetCore.EasyTesting
{
    public static class HttpResponseMessageAssertionHelper
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void ShouldBeOk(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.OK);
        }

        public static async Task<TResponse> ShouldBeOk<TResponse>(this HttpResponseMessage message)
        {
            return await message.ShouldBeCodeWithBody<TResponse>(HttpStatusCode.OK);
        }

        public static async Task ShouldBeOk<TResponse>(this HttpResponseMessage message, Action<TResponse> assertion)
        {
            assertion(await message.ShouldBeOk<TResponse>());
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

        public static async Task<TResponse> ShouldBeCodeWithBody<TResponse>(this HttpResponseMessage message,
            HttpStatusCode code)
        {
            message.ShouldBe(code);
            return await message.ReadJsonBodyAsync<TResponse>();
        }

        public static async Task<TResponse> ReadJsonBodyAsync<TResponse>(this HttpResponseMessage message)
        {
            var body = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(body, JsonSerializerOptions);
        }
    }
}