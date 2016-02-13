using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ServerStack.Dispatch;
using ServerStack.Serialization;
using ServerStack.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DispatcherServiceCollectionExtensions
    {
        public static IServiceCollection AddDispatcher<TObject>(this IServiceCollection services)
        {
            services.AddSingleton<Dispatcher<TObject>>();
            services.AddSingleton<IObservable<Frame<TObject>>>(sp => sp.GetService<Dispatcher<TObject>>());
            return services;
        }
    }
}
