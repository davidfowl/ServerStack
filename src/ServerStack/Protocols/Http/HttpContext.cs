using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Protocols.Http
{
    public class HttpContext
    {
        private readonly IFeatureCollection _features;

        public HttpContext(IFeatureCollection features)
        {
            _features = features;
        }
    }
}
