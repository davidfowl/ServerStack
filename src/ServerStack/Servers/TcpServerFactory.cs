using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ServerStack.Servers
{
    public class TcpServerFactory : IServerFactory
    {
        public IServer CreateServer(IConfiguration configuration)
        {
            int port;
            if (!Int32.TryParse(configuration["port"], out port))
            {
                port = 3045;
            }
            return new TcpServer(port);
        }
    }
}
