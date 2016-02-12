using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServerStack;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;
using ServerStack.Serialization;
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
            services.AddJsonEncoders();

            services.AddSingleton(typeof(IFrameEncoder<NewLineMessage>), typeof(NewLineEncoder));
            services.AddSingleton(typeof(IFrameDecoder<NewLineMessage>), typeof(NewLineDecoder));
            services.AddSingleton(typeof(IFrameHandler<NewLineMessage>), typeof(NewLineHandler));
        }

        public void Configure(IApplicationBuilder<TcpContext> app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseLogging();

            app.UseDispatcher<NewLineMessage>();
        }
    }

    public struct NewLineMessage
    {
        public string Line { get; set; }
    }

    public class NewLineHandler : IFrameHandler<NewLineMessage>
    {
        public Task<object> OnFrame(NewLineMessage value)
        {
            var result = new JObject();

            result["message"] = value.Line;

            return Task.FromResult<object>(result);
        }
    }

    public class NewLineEncoder : IFrameEncoder<NewLineMessage>
    {
        public Task Encode(Stream output, NewLineMessage value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.Line + Environment.NewLine);

            return output.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    public class NewLineDecoder : IFrameDecoder<NewLineMessage>
    {
        public async Task<NewLineMessage> Decode(Stream input)
        {
            var sr = new StreamReader(input);
            var line = await sr.ReadLineAsync();

            if (string.IsNullOrEmpty(line))
            {
                return default(NewLineMessage);
            }
            return new NewLineMessage
            {
                Line = line
            };
        }
    }
}
