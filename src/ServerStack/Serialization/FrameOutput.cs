using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;


namespace ServerStack.Serialization
{
    public class FrameOutput : IFrameOutput
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, CacheEntry> _cache = new ConcurrentDictionary<Type, CacheEntry>();

        public FrameOutput(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task WriteAsync(Stream stream, object value)
        {
            if (value == null)
            {
                return Task.FromResult(0);
            }

            Type type = value.GetType();
            var entry = _cache.GetOrAdd(type, t => CreateCacheEntry(t));

            return entry.Encode(entry.Encoder, stream, value);
        }

        private CacheEntry CreateCacheEntry(Type type)
        {
            var encoderType = typeof(IStreamEncoder<>).MakeGenericType(type);

            // Func<object, Stream, object, Task> callback = (encoder, body, value) =>
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
            var lambda = Expression.Lambda<Func<object, Stream, object, Task>>(encodeCall, encoderParam, bodyParam, valueParam);

            var entry = new CacheEntry
            {
                Encode = lambda.Compile(),
                Encoder = _serviceProvider.GetService(encoderType)
            };

            return entry;
        }

        private struct CacheEntry
        {
            public Func<object, Stream, object, Task> Encode;
            public object Encoder;
        }
    }
}
