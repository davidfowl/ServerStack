using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerStack;
using ServerStack.Dispatch;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;

namespace ChatSample
{
    public class BasicChat
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodec<ChatMessage, ChatMessageEncoder, ChatMessageDecoder>();

            services.AddSingleton<ChatMessageHandler>();
            services.AddSingleton(typeof(IFrameHandler<ChatMessage>), sp => sp.GetService<ChatMessageHandler>());
            services.AddSingleton(typeof(IObservable<ChatMessage>), sp => sp.GetService<ChatMessageHandler>());
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
