using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerStack;
using ServerStack.Protocols.Tcp;
using ServerStack.Servers;

namespace ChatSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new ServerHostBuilder<TcpContext>()
                    .UseSetting("server.address", "tcp://127.0.0.1:1335")
                    .UseServer<TcpServerFactory>()
                    .UseStartup<BasicChat>()
                    .Build();

            host.Run();
        }
    }
}
