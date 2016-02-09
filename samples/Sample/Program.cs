using System;
using JsonRPC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerStack;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;
using ServerStack.Servers;

namespace Sample
{
    public class Program
    {
        public static void Main()
        {
            var host = new ServerHostBuilder<TcpContext>()
                    .UseSetting("server.address", "tcp://127.0.0.1:1335")
                    .UseServer<TcpServerFactory>()
                    .UseStartup<TcpStartup>()
                    .Build();

            host.Run();
        }
    }

    public class TcpStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsonRPC();
        }

        public void Configure(IApplicationBuilder<TcpContext> app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseLogging();

            app.UseJsonRPC();
        }
    }
}
