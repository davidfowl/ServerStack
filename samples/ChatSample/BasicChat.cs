using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerStack;
using ServerStack.Dispatch;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;
using ServerStack.Serialization;

namespace ChatSample
{
    public class BasicChat
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodec<ChatMessage, ChatMessageEncoder, ChatMessageDecoder>();

            services.AddDispatcher<ChatMessage>();
        }

        public void Configure(IApplicationBuilder<TcpContext> app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseLogging();

            app.UseChat();

            app.UseDispatcher<ChatMessage>();
        }
    }
}
