using System;
using ServerStack.Dispatch;
using ServerStack.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DispatcherServiceCollectionExtensions
    {
        public static IServiceCollection AddDispatcher<TObject>(this IServiceCollection services)
        {
            services.AddSingleton<IObservable<Frame<TObject>>, DispatcherObservable<TObject>>();
            return services;
        }
    }
}
