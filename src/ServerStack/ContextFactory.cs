using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public class ContextFactory<TContext> : IContextFactory<TContext>
    {
        public TContext CreateContext(IFeatureCollection features)
        {
            return features.Get<TContext>();
        }
    }
}
