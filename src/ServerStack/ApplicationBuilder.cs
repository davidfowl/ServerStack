using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public class ApplicationBuilder<TContext> : IApplicationBuilder<TContext>
    {
        public IServiceProvider ApplicationServices { get; set; }

        public IDictionary<string, object> Properties { get; }

        public IFeatureCollection ServerFeatures { get; private set; }

        public Func<TContext, Task> Build()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder<TContext> New()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder<TContext> Use(Func<Func<TContext, Task>, Func<TContext, Task>> middleware)
        {
            throw new NotImplementedException();
        }
    }
}
