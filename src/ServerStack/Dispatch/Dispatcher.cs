using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServerStack.Dispatch;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class Dispatcher<T>
    {
        private readonly IFrameDecoder<T> _decoder;
        private readonly IFrameHandler<T> _frameHandler;
        private readonly ILogger<Dispatcher<T>> _logger;

        public Dispatcher(ILogger<Dispatcher<T>> logger,
                          IFrameDecoder<T> decoder,
                          IFrameHandler<T> frameHandler)
        {
            _logger = logger;
            _decoder = decoder;
            _frameHandler = frameHandler;
        }

        public async Task Invoke(Stream stream)
        {
            while (true)
            {
                try
                {
                    // Read some data, if the decoder doesn't consume any then stop
                    var frames = new List<T>();
                    await _decoder.Decode(stream, frames);

                    if (frames.Count == 0)
                    {
                        break;
                    }

                    foreach (var frame in frames)
                    {
                        await _frameHandler.OnFrame(stream, frame);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to process frame", ex);
                    break;
                }
            }
        }
    }
}
