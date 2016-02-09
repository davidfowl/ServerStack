using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using ServerStack;
using ServerStack.Protocols.Tcp;

namespace JsonRPC
{
    public static class JsonRPCMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseJsonRPC(this IApplicationBuilder<TcpContext> app)
        {
            return app.UseJsonRPC(new JsonSerializerSettings());
        }

        public static IApplicationBuilder<TcpContext> UseJsonRPC(this IApplicationBuilder<TcpContext> app, JsonSerializerSettings settings)
        {
            return app.Use(next =>
            {
                var handler = new JsonRPCHandler(settings, app.ApplicationServices);

                foreach (var endpoint in app.ApplicationServices.GetServices<RpcEndpoint>())
                {
                    handler.Bind(endpoint.GetType());
                }

                // REVIEW: Who owns the stream?
                // REVIEW: Should this call next if the data over the channel doesn't yield a method?
                return ctx => handler.StartAsync(ctx.Body);
            });
        }
    }
}
