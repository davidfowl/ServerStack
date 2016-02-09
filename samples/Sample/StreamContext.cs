using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Sample;

namespace ServerStack.Protocols
{
    internal class StreamContext : IStreamContext
    {
        private readonly Stream _body;
        private readonly IStreamEncoder _encoder;

        public StreamContext(Stream body, IStreamEncoder encoder)
        {
            _body = body;
            _encoder = encoder;
        }

        public Task WriteAsync(object value)
        {
            return _encoder.WriteAsync(_body, value);
        }
    }
}