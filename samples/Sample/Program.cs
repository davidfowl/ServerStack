using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Middleware;
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
            var host = new HostBuilder<TcpContext>()
                    .UseServer<TcpServerFactory>()
                    .UseSetting("server.address", "tcp://127.0.0.1:22")
                    .UseStartup<TcpStartup>()
                    .Build();

            host.Run();
        }
    }

    public class TcpStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MyType>();
        }

        public void Configure(IApplicationBuilder<TcpContext> app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseExceptionHandler(ex =>
            {
                Console.WriteLine("Exception was thrown!: " + ex);
            });

            // app.UseTls(new X509Certificate2("dotnetty.com.pfx", "password"));

            app.UseJsonRPC<MyType>();
        }
    }

    public class MyType
    {
        private readonly ILogger<MyType> _logger;
        public MyType(ILogger<MyType> logger)
        {
            _logger = logger;
        }

        public int Increment(int value)
        {
            _logger.LogInformation("Received " + value);

            return value + 1;
        }
    }
}
