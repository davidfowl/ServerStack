using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ServerStack.Features;
using ServerStack.Middleware;
using ServerStack.Servers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServerStack
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                    .UseServer<TcpServerFactory>()
                    .UseSetting("port", "4000")
                    .UseStartup<Startup>()
                    .Build();

            host.Run();
        }
    }

    public class Startup
    {
        public void Configure(IApplicationBuilder<IFeatureCollection> app)
        {
            app.UseExceptionHandler(ex =>
            {
                Console.WriteLine("Exception was thrown!: " + ex);
            });

            app.UseTls(new X509Certificate2("dotnetty.com.pfx", "password"));

            app.Run(async ctx =>
            {
                var bytes = Encoding.UTF8.GetBytes("Hello World");
                await ctx.Get<IConnectionFeature>().Body.WriteAsync(bytes, 0, bytes.Length);
            });
        }
    }
}
