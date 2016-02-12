using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServerStack.Features;

namespace ServerStack.Servers
{
    public class TcpServer : IServer
    {
        public IFeatureCollection Features { get; }

        private readonly TcpListener _listener;
        private readonly ILogger _logger;

        public TcpServer(IPEndPoint endPoint, ILoggerFactory loggerFactory)
        {
            _listener = new TcpListener(endPoint);
            _logger = loggerFactory.CreateLogger<TcpServer>();
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

                _logger.LogVerbose("Accepted connection {connection}", client.Client.RemoteEndPoint);

                var ignore = Task.Run(async () =>
                {
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
                    finally
                    {
                        _logger.LogVerbose("Connection {connection} closed", client.Client.RemoteEndPoint);
                    }
                });
            }
        }
    }
}
