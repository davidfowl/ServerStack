using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder<TContext> Run<TContext>(this IApplicationBuilder<TContext> app, Func<TContext, Task> callback)
        {
            return app.Use(next => ctx => callback(ctx));
        }
    }

    public interface IApplicationBuilder<TContext>
    {
        IServiceProvider ApplicationServices { get; set; }

        IFeatureCollection ServerFeatures { get; }

        IDictionary<string, object> Properties { get; }

        IApplicationBuilder<TContext> Use(Func<Func<TContext, Task>, Func<TContext, Task>> middleware);
        
        IApplicationBuilder<TContext> New();

        Func<TContext, Task> Build();
    }
}
