using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsonRPC;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using ServerStack.Protocols;
using ServerStack.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonRPCServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonRPC(this IServiceCollection services)
        {
            var libraries = PlatformServices.Default.LibraryManager.GetReferencingLibraries(typeof(RpcEndpoint).Assembly.GetName().Name);

            foreach (var library in libraries)
            {
                foreach (var an in library.Assemblies)
                {
                    var asm = Assembly.Load(an);
                    foreach (var type in asm.GetTypes())
                    {
                        if (typeof(RpcEndpoint) != type && typeof(RpcEndpoint).IsAssignableFrom(type))
                        {
                            services.AddTransient(typeof(RpcEndpoint), type);
                            services.AddTransient(type);
                        }
                    }
                }
            }

            services.AddJsonEncoders();
            services.AddSingleton<IFrameHandler<JObject>, JsonRPCHandler>();
            return services;
        }
    }

}
