using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public interface IApplication<TContext>
    {
        TContext CreateContext(IFeatureCollection features);

        Task ProcessRequestAsync(TContext context);

        void DisposeContext(TContext context, Exception exception);
    }
}
