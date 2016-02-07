using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServerStack.Protocols.Tcp
{
    public class TcpContext
    {
        public IPAddress RemoteIpAddress { get; set; }
        public IPAddress LocalIpAddress { get; set; }
        public int RemotePort { get; set; }
        public int LocalPort { get; set; }

        public Stream Body { get; set; }
    }
}
