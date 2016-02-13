using System;
using System.IO;
using ServerStack.Infrastructure;

namespace ServerStack.Serialization
{
    public class DefaultOutputProducer : IOutputProducer, IDisposable
    {
        private readonly Stream _body;
        private readonly IFrameOutput _output;
        private readonly TaskQueue _queue = new TaskQueue(10000);

        public DefaultOutputProducer(IFrameOutput output, Stream body)
        {
            _output = output;
            _body = body;
        }

        public void Dispose()
        {
            _queue.Drain().GetAwaiter().GetResult();
        }

        public void Produce(object value)
        {
            // Fire and forget
            // REVIEW: How to errors get handled?
            _queue.Enqueue(() => _output.WriteAsync(_body, value));
        }
    }
}
