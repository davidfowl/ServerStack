using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ServerStack.Servers
{
    public class TcpServerFactory : IServerFactory
    {
        public IServer CreateServer(IConfiguration configuration)
        {
            var ip = new IPEndPoint(IPAddress.Loopback, 5000);
            var address = configuration["server.address"];
            if (!string.IsNullOrEmpty(address))
            {
                var uri = new Uri(address);
                if (!string.Equals(uri.Scheme, "tcp", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Invalid scheme {uri.Scheme}");
                }

                ip = new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port);
            }
            return new TcpServer(ip);
        }
    }
}
