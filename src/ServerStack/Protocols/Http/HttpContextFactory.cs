using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Protocols.Http
{
    public class HttpContextFactory : IContextFactory<HttpContext>
    {
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new HttpContext(features);
        }
    }
}
