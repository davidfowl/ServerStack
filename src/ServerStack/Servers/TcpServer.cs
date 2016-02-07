using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerStack.Features;

namespace ServerStack.Servers
{
    public class TcpServer : IServer
    {
        public IFeatureCollection Features { get; }

        private readonly TcpListener _listener;

        public TcpServer(IPEndPoint endPoint)
        {
            _listener = new TcpListener(endPoint);
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        public async void Start<TContext>(IApplication<TContext> application)
        {
            _listener.Start();

            // Async void is bad
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var fc = new FeatureCollection();
                fc.Set<IConnectionFeature>(new ConnectionFeature
                {
                    Body = client.GetStream()
                });

                var context = application.CreateContext(fc);

                try
                {
                    await application.ProcessRequestAsync(context);

                    application.DisposeContext(context, null);
                }
                catch (Exception ex)
                {
                    application.DisposeContext(context, ex);
                }
            }
        }
    }
}
