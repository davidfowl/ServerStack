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
        private readonly IOutputProducerFactory _producerFactory;
        private readonly IObservable<Frame<ChatMessage>> _observable;


        public ChatMiddleware(Func<TcpContext, Task> next,
                              IOutputProducerFactory producerFactory,
                              IObservable<Frame<ChatMessage>> observable)
        {
            _next = next;
            _producerFactory = producerFactory;
            _observable = observable;
        }

        public async Task Invoke(TcpContext context)
        {
            var producer = _producerFactory.Create(context.Body);

            try
            {
                using (_observable.Subscribe(new ChatClient(producer)))
                {
                    await _next(context);
                }
            }
            finally
            {
                (producer as IDisposable)?.Dispose();
            }
        }

        private class ChatClient : IObserver<Frame<ChatMessage>>
        {
            private readonly IOutputProducer _output;

            public ChatClient(IOutputProducer output)
            {
                _output = output;
            }

            public void OnCompleted()
            {

            }

            public void OnError(Exception error)
            {

            }

            public void OnNext(Frame<ChatMessage> value)
            {
                _output.Produce(value.Data);
            }
        }
    }

    public static class UserTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseChat(this IApplicationBuilder<TcpContext> app)
        {
            var output = app.ApplicationServices.GetRequiredService<IOutputProducerFactory>();
            var observable = app.ApplicationServices.GetRequiredService<IObservable<Frame<ChatMessage>>>();

            return app.Use(next => new ChatMiddleware(next, output, observable).Invoke);
        }

    }
}
