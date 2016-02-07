using Newtonsoft.Json;
using ServerStack;
using ServerStack.Protocols.Tcp;

namespace Sample.Middleware
{
    public static class JsonRPCMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseJsonRPC<T>(this IApplicationBuilder<TcpContext> app) where T : class
        {
            return app.UseJsonRPC<T>(new JsonSerializerSettings());
        }

        public static IApplicationBuilder<TcpContext> UseJsonRPC<T>(this IApplicationBuilder<TcpContext> app, JsonSerializerSettings settings) where T : class
        {
            return app.Use(next =>
            {
                var handler = new JsonRPCHandler(settings, app.ApplicationServices);
                handler.Bind<T>();

                // REVIEW: Who owns the stream?
                // REVIEW: Should this call next if the data over the channel doesn't yield a method?
                return ctx => handler.StartAsync(ctx.Body);
            });
        }
    }
}
