using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public class Dispatcher<T> : IDispatcher<T>
    {
        private readonly IFrameDecoder<T> _decoder;
        private readonly ILogger<Dispatcher<T>> _logger;
        private readonly IOutputProducerFactory _producerFactory;

        public event Action<IOutputProducer, T> Callback;

        public event Action<Exception> OnError;

        public event Action OnCompleted;

        public Dispatcher(ILogger<Dispatcher<T>> logger,
                          IFrameDecoder<T> decoder,
                          IOutputProducerFactory producerFactory)
        {
            _logger = logger;
            _decoder = decoder;
            _producerFactory = producerFactory;
        }

        public async Task Invoke(Stream stream)
        {
            var producer = _producerFactory.Create(stream);

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
                        Callback(producer, frame);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex);

                    _logger.LogError("Failed to process frame", ex);
                    break;
                }
            }

            OnCompleted();

            (producer as IDisposable)?.Dispose();
        }
    }
}
