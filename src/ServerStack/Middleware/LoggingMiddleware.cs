using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerStack.Protocols.Tcp;

namespace ServerStack.Middleware
{
    public class LoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly Func<TcpContext, Task> _next;

        public LoggingMiddleware(Func<TcpContext, Task> next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        }

        public Task Invoke(TcpContext context)
        {
            context.Body = new LoggingStream(context.Body, _logger);

            return _next(context);
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseLogging(this IApplicationBuilder<TcpContext> app)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            return app.Use(next =>
            {
                return new LoggingMiddleware(next, loggerFactory).Invoke;
            });
        }
    }
}
