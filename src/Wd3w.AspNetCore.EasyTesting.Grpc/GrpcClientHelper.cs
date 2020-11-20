using System;
using Grpc.Net.Client;

namespace Wd3w.AspNetCore.EasyTesting.Grpc
{
    public static class GrpcClientHelper
    {
        /// <summary>
        ///     Create grpc client for T
        /// </summary>
        /// <param name="sut"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateClient<T>(this SystemUnderTest sut)
        {
            var channel = CreateGrpcChannel(sut);
            return (T) Activator.CreateInstance(typeof(T), channel);
        }

        /// <summary>
        ///     Create grpc channel for creating grpc service client
        /// </summary>
        /// <param name="sut"></param>
        /// <returns></returns>
        public static GrpcChannel CreateGrpcChannel(this SystemUnderTest sut)
        {
            var httpClient = sut.CreateDefaultClient(new ResponseVersionHandler());

            return GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = httpClient
            });
        }
    }
}