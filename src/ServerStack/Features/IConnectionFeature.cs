using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServerStack.Features
{

    public interface IConnectionFeature
    {
        IPAddress RemoteIpAddress { get; set; }
        IPAddress LocalIpAddress { get; set; }
        int RemotePort { get; set; }
        int LocalPort { get; set; }

        Stream Body { get; set; }
    }

    public class ConnectionFeature : IConnectionFeature
    {
        public IPAddress RemoteIpAddress { get; set; }
        public IPAddress LocalIpAddress { get; set; }
        public int RemotePort { get; set; }
        public int LocalPort { get; set; }
        public Stream Body { get; set; }
    }
}
