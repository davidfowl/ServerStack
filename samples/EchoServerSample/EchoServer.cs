using Microsoft.Extensions.Logging;
using ServerStack;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;

namespace EchoServerSample
{
    public class EchoServer
    {
        public void Configure(IApplicationBuilder<TcpContext> app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseLogging();

            app.Run(context =>
            {
                return context.Body.CopyToAsync(context.Body);
            });
        }
    }
}
