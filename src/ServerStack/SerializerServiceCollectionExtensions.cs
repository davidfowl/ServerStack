using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ServerStack.Serialization;
using ServerStack.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonCodec<TObject>(this IServiceCollection services)
        {
            return services.AddCodec<TObject, JsonEncoder<TObject>, JsonDecoder<TObject>>();
        }

        public static IServiceCollection AddCodec<TObject, TEncoder, TDecoder>(this IServiceCollection services)
                where TEncoder : class, IFrameEncoder<TObject>
                where TDecoder : class, IFrameDecoder<TObject>
        {
            services.AddEncoder<TObject, TEncoder>();
            services.AddDecoder<TObject, TDecoder>();
            return services;
        }

        public static IServiceCollection AddEncoder<TObject, TEncoder>(this IServiceCollection services)
                where TEncoder : class, IFrameEncoder<TObject>
        {
            services.AddSingleton<IFrameEncoder<TObject>, TEncoder>();
            return services;
        }

        public static IServiceCollection AddDecoder<TObject, TDecoder>(this IServiceCollection services)
                where TDecoder : class, IFrameDecoder<TObject>
        {
            services.AddSingleton<IFrameDecoder<TObject>, TDecoder>();
            return services;
        }
    }
}
