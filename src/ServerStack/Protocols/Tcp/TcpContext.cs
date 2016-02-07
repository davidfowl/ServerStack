using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ServerStack.Features;

namespace ServerStack.Protocols.Tcp
{
    public class TcpContext
    {
        private readonly IFeatureCollection _features;

        public TcpContext(IFeatureCollection features)
        {
            _features = features;
        }

        public IPAddress RemoteIpAddress { get; set; }
        public IPAddress LocalIpAddress { get; set; }
        public int RemotePort { get; set; }
        public int LocalPort { get; set; }

        public Stream Body
        {
            get
            {
                return _features.Get<IConnectionFeature>().Body;
            }
            set
            {
                {
                    _features.Get<IConnectionFeature>().Body = value;
                }
            }
        }
    }
}
