using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServerStack.Serialization;

namespace ServerStack
{
    public class Dispatcher<T>
    {
        private readonly IFrameDecoder<T> _decoder;
        private readonly IFrameHandler<T> _frameHandler;
        private readonly IFrameOutput _frameoutput;
        private readonly ILogger<Dispatcher<T>> _logger;

        public Dispatcher(ILogger<Dispatcher<T>> logger,
                          IFrameDecoder<T> decoder,
                          IFrameHandler<T> frameHandler,
                          IFrameOutput encoder)
        {
            _logger = logger;
            _decoder = decoder;
            _frameoutput = encoder;
            _frameHandler = frameHandler;
        }

        public async Task Invoke(Stream input)
        {
            while (true)
            {
                try
                {
                    T frame = await _decoder.Decode(input);
                    var value = await _frameHandler.OnFrame(frame);
                    await _frameoutput.WriteAsync(input, value);
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
