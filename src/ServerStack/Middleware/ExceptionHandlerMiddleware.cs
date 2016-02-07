using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Middleware
{
    public class ExceptionHandlerMiddleware<TContext>
    {
        private readonly Func<TContext, Task> _next;
        private readonly Action<Exception> _handler;
        public ExceptionHandlerMiddleware(Func<TContext, Task> next, Action<Exception> handler)
        {
            _next = next;
            _handler = handler;
        }

        public async Task Invoke(TContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _handler(ex);
            }
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder<TContext> UseExceptionHandler<TContext>(this IApplicationBuilder<TContext> app, Action<Exception> handler)
        {
            return app.Use(next => ctx => new ExceptionHandlerMiddleware<TContext>(next, handler).Invoke(ctx));
        }
    }
}
