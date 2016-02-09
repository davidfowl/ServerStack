using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sample;
using ServerStack.Protocols.Tcp;
using Microsoft.Extensions.DependencyInjection;
using ServerStack.Protocols;
using Microsoft.Extensions.Logging;

namespace ServerStack.Middleware
{
    public class FramingMiddleware<T>
    {
        private readonly Func<TcpContext, Task> _next;
        private readonly IStreamDecoder<T> _decoder;
        private readonly IFrameHandler<T> _frameHandler;
        private readonly IStreamEncoder _encoder;
        private readonly ILogger<FramingMiddleware<T>> _logger;

        public FramingMiddleware(Func<TcpContext, Task> next,
                                 ILogger<FramingMiddleware<T>> logger,
                                 IStreamDecoder<T> decoder,
                                 IFrameHandler<T> frameHandler,
                                 IStreamEncoder encoder)
        {
            _logger = logger;
            _next = next;
            _decoder = decoder;
            _encoder = encoder;
            _frameHandler = frameHandler;
        }

        public async Task Invoke(TcpContext context)
        {
            while (true)
            {
                try
                {
                    T frame;
                    bool decoded = await _decoder.TryDecode(context.Body, out frame);

                    // Nothing was read
                    if (!decoded)
                    {
                        break;
                    }

                    await _frameHandler.OnFrame(new StreamContext(context.Body, _encoder), frame);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to process frame", ex);
                    break;
                }
            }
        }
    }

    public static class MessagingMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseFraming<T>(this IApplicationBuilder<TcpContext> app)
        {
            return app.Use(next =>
            {
                var decoder = app.ApplicationServices.GetRequiredService<IStreamDecoder<T>>();
                var handler = app.ApplicationServices.GetRequiredService<IFrameHandler<T>>();
                var encoder = app.ApplicationServices.GetRequiredService<IStreamEncoder>();
                var logger = app.ApplicationServices.GetRequiredService<ILogger<FramingMiddleware<T>>>();

                return new FramingMiddleware<T>(next, logger, decoder, handler, encoder).Invoke;
            });
        }
    }
}
