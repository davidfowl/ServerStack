using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Protocols.Tcp
{
    public class TcpContextFactory : IContextFactory<TcpContext>
    {
        public TcpContext CreateContext(IFeatureCollection features)
        {
            return new TcpContext(features);
        }
    }
}
