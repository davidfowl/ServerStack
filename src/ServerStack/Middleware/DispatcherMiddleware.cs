using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServerStack.Dispatch;
using ServerStack.Protocols.Tcp;

namespace ServerStack.Middleware
{
    public class DispatcherMiddleware<T>
    {
        private readonly Func<TcpContext, Task> _next;
        private readonly Dispatcher<T> _dispatcher;

        public DispatcherMiddleware(Func<TcpContext, Task> next,
                                    Dispatcher<T> dispatcher)
        {
            _next = next;
            _dispatcher = dispatcher;
        }

        public async Task Invoke(TcpContext context)
        {
            await _dispatcher.Invoke(context.Body);
            await _next(context);
        }
    }

    public static class DispatcherMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseDispatcher<T>(this IApplicationBuilder<TcpContext> app)
        {
            return app.Use(next =>
            {
                var dispatcher = app.ApplicationServices.GetRequiredService<Dispatcher<T>>();

                return new DispatcherMiddleware<T>(next, dispatcher).Invoke;
            });
        }
    }
}
