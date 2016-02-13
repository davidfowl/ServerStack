using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServerStack;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;

namespace JsonRPCSample
{
    public class JsonRPCStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsonRPC();
        }

        public void Configure(IApplicationBuilder<TcpContext> app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseJsonRpc();
        }
    }
}