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
            services.AddSingleton<IStreamDecoder<JObject>, JsonDecoder>();
            services.AddSingleton<IStreamEncoder<JObject>, JsonEncoder>();
            return services;
        }
    }
}
