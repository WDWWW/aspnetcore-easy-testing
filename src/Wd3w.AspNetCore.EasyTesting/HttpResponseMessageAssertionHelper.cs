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

        /// <summary>
        ///     Verify response message status code is ok.
        /// </summary>
        /// <param name="message"></param>
        public static void ShouldBeOk(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.OK);
        }

        /// <summary>
        ///      Verify response message status code is ok and deserialization json as TResponse type.
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static async Task<TResponse> ShouldBeOk<TResponse>(this HttpResponseMessage message)
        {
            return await message.ShouldBeCodeWithBody<TResponse>(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Verify response message status code is ok and deserialization response json as TResponse and provide to assertion action.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="assertion"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static async Task ShouldBeOk<TResponse>(this HttpResponseMessage message, Action<TResponse> assertion)
        {
            assertion(await message.ShouldBeOk<TResponse>());
        }

        /// <summary>
        ///     Verify response code is nocontent.
        /// </summary>
        /// <param name="message"></param>
        public static void ShouldBeNoContent(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///     Verify response code is accepted.
        /// </summary>
        /// <param name="message"></param>
        public static void ShouldBeAccepted(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.Accepted);
        }

        /// <summary>
        ///     Verify response code is bad request.
        /// </summary>
        /// <param name="message"></param>
        public static void ShouldBeBadRequest(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.BadRequest);
        }

        /// <summary>
        ///     Verify response code is forbidden.
        /// </summary>
        /// <param name="message"></param>
        public static void ShouldBeForbidden(this HttpResponseMessage message)
        {
            message.ShouldBe(HttpStatusCode.Forbidden);
        }

        /// <summary>
        ///     Verify response code.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public static void ShouldBe(this HttpResponseMessage message, HttpStatusCode code)
        {
            message.StatusCode.Should().Be(code);
        }

        /// <summary>
        ///     Verify response code and deserialize json response.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static async Task<TResponse> ShouldBeCodeWithBody<TResponse>(this HttpResponseMessage message,
            HttpStatusCode code)
        {
            message.ShouldBe(code);
            return await message.ReadJsonBodyAsync<TResponse>();
        }

        /// <summary>
        ///     Deserialize json body content from response message.
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static async Task<TResponse> ReadJsonBodyAsync<TResponse>(this HttpResponseMessage message)
        {
            var body = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(body, JsonSerializerOptions);
        }
    }
}