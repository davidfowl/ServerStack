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
        public static IServiceCollection AddJsonEncoders(this IServiceCollection services)
        {
            services.AddSingleton<IFrameDecoder<JObject>, JsonDecoder>();
            services.AddSingleton<IFrameEncoder<JObject>, JsonEncoder>();
            return services;
        }
    }
}
