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
    public interface IStreamContext
    {
        Task WriteAsync(object value);
    }

    public interface IFrameHandler<TInput>
    {
        Task OnFrame(IStreamContext context, TInput value);
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

    public class StreamEncoder : IStreamEncoder
    {
        private readonly IServiceProvider _sp;
        private readonly ConcurrentDictionary<Type, Func<object, Stream, object, Task>> _cache = new ConcurrentDictionary<Type, Func<object, Stream, object, Task>>();

        public StreamEncoder(IServiceProvider sp)
        {
            _sp = sp;
        }

        public Task WriteAsync(Stream stream, object value)
        {
            if (value == null)
            {
                return Task.FromResult(0);
            }

            Type type = value.GetType();
            var encoderType = typeof(IStreamEncoder<>).MakeGenericType(type);

            var func = _cache.GetOrAdd(type, key =>
            {

                // Func<object, object, Task> f = (encoder, body, value) =>
                // { 
                //   return ((IStreamEncoder<T>)encoder).Encode(body, (T)value);
                // }

                var encoderParam = Expression.Parameter(typeof(object), "encoder");
                var bodyParam = Expression.Parameter(typeof(Stream), "body");
                var valueParam = Expression.Parameter(typeof(object), "value");

                var encoderCast = Expression.Convert(encoderParam, encoderType);
                var valueCast = Expression.Convert(valueParam, type);
                var encode = encoderType.GetMethod("Encode", BindingFlags.Public | BindingFlags.Instance);
                var encodeCall = Expression.Call(encoderCast, encode, bodyParam, valueCast);

                return Expression.Lambda<Func<object, Stream, object, Task>>(encodeCall, encoderParam, bodyParam, valueParam).Compile();
            });

            object encoder = _sp.GetService(encoderType);
            if (encoder != null)
            {
                return func(encoder, stream, value);
            }
            return Task.FromResult(0);
        }
    }

}
