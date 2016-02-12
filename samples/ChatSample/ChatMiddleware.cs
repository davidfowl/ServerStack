using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServerStack;
using ServerStack.Protocols.Tcp;
using ServerStack.Serialization;

namespace ChatSample
{
    public class ChatMiddleware
    {
        private readonly Func<TcpContext, Task> _next;
        private readonly IFrameOutput _output;
        private readonly IObservable<ChatMessage> _observable;


        public ChatMiddleware(Func<TcpContext, Task> next, IFrameOutput output, IObservable<ChatMessage> observable)
        {
            _next = next;
            _output = output;
            _observable = observable;
        }

        public async Task Invoke(TcpContext context)
        {
            using (_observable.Subscribe(new ChatClient(context, _output)))
            {
                await _next(context);
            }
        }

        private class ChatClient : IObserver<ChatMessage>
        {
            private readonly TcpContext _context;
            private readonly IFrameOutput _output;

            public ChatClient(TcpContext context, IFrameOutput output)
            {
                _context = context;
                _output = output;
            }

            public void OnCompleted()
            {

            }

            public void OnError(Exception error)
            {

            }

            public void OnNext(ChatMessage value)
            {
                _output.WriteAsync(_context.Body, value);
            }
        }
    }

    public static class UserTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseChat(this IApplicationBuilder<TcpContext> app)
        {
            var output = app.ApplicationServices.GetRequiredService<IFrameOutput>();
            var observable = app.ApplicationServices.GetRequiredService<IObservable<ChatMessage>>();

            return app.Use(next => new ChatMiddleware(next, output, observable).Invoke);
        }

    }
}
