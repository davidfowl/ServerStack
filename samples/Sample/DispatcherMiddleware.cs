using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerStack.Protocols;
using ServerStack.Protocols.Tcp;
using ServerStack.Serialization;

namespace ServerStack.Middleware
{
    public class DispatcherMiddleware<T>
    {
        private readonly Func<TcpContext, Task> _next;
        private readonly IStreamDecoder<T> _decoder;
        private readonly IFrameHandler<T> _frameHandler;
        private readonly IFrameOutput _frameoutput;
        private readonly ILogger<DispatcherMiddleware<T>> _logger;

        public DispatcherMiddleware(Func<TcpContext, Task> next,
                                    ILogger<DispatcherMiddleware<T>> logger,
                                    IStreamDecoder<T> decoder,
                                    IFrameHandler<T> frameHandler,
                                    IFrameOutput encoder)
        {
            _next = next;
            _logger = logger;
            _decoder = decoder;
            _frameoutput = encoder;
            _frameHandler = frameHandler;
        }

        public async Task Invoke(TcpContext context)
        {
            while (true)
            {
                try
                {
                    T frame = await _decoder.Decode(context.Body);
                    var value = await _frameHandler.OnFrame(frame);
                    await _frameoutput.WriteAsync(context.Body, value);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to process frame", ex);
                    break;
                }
            }
        }
    }

    public static class DispatcherMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseDispatcher<T>(this IApplicationBuilder<TcpContext> app)
        {
            return app.Use(next =>
            {
                var decoder = app.ApplicationServices.GetRequiredService<IStreamDecoder<T>>();
                var handler = app.ApplicationServices.GetRequiredService<IFrameHandler<T>>();
                var output = app.ApplicationServices.GetRequiredService<IFrameOutput>();
                var logger = app.ApplicationServices.GetRequiredService<ILogger<DispatcherMiddleware<T>>>();

                return new DispatcherMiddleware<T>(next, logger, decoder, handler, output).Invoke;
            });
        }
    }
}
