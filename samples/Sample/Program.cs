using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            var host = new HostBuilder()
                    .UseServer<TcpServerFactory>()
                    .UseSetting("port", "4000")
                    .UseStartup<TcpStartup>()
                    .Build();

            host.Run();
        }
    }

    public class TcpStartup
    {
        public void Configure(ITcpApplicationBuilder app)
        {
            app.UseExceptionHandler(ex =>
            {
                Console.WriteLine("Exception was thrown!: " + ex);
            });

            app.UseTls(new X509Certificate2("dotnetty.com.pfx", "password"));

            app.Run(async ctx =>
            {
                var bytes = Encoding.UTF8.GetBytes("Hello World");
                await ctx.Body.WriteAsync(bytes, 0, bytes.Length);
            });
        }
    }
}
