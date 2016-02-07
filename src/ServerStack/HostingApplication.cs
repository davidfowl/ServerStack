using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public class HostingApplication<TContext> : IApplication<TContext>
    {
        private readonly Func<TContext, Task> _pipeline;
        private readonly IContextFactory<TContext> _contextFactory;

        public HostingApplication(IContextFactory<TContext> contextFactory, Func<TContext, Task> pipeline)
        {
            _contextFactory = contextFactory;
            _pipeline = pipeline;
        }

        public TContext CreateContext(IFeatureCollection features)
        {
            return _contextFactory.CreateContext(features);
        }

        public void DisposeContext(TContext context, Exception exception)
        {

        }

        public Task ProcessRequestAsync(TContext context)
        {
            return _pipeline(context);
        }
    }
}
