using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ServerStack.Protocols
{
    public interface IFrameHandler<TInput>
    {
        Task<object> OnFrame(TInput value);
    }

    public interface IStreamDecoder<TInput>
    {
        Task<bool> TryDecode(Stream input, out TInput frame);
    }

    public interface IStreamEncoder<TOutput>
    {
        Task Encode(Stream output, TOutput value);
    }

    public interface IStreamEncoder
    {
        Task WriteAsync(Stream output, object value);
    }
}
