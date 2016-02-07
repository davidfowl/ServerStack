using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public class Host<TContext> : IHost<TContext>
    {
        private readonly Func<TContext, Task> _pipeline;
        private readonly IServer _server;
        private readonly IContextFactory<TContext> _contextFactory;

        public Host(IServer server, IServiceProvider services, IContextFactory<TContext> contextFactory, Func<TContext, Task> pipeline)
        {
            _server = server;
            ServerFeatures = server.Features;
            Services = services;
            _contextFactory = contextFactory;
            _pipeline = pipeline;
        }

        public IFeatureCollection ServerFeatures { get; }

        public IServiceProvider Services { get; }

        public void Dispose()
        {
            _server.Dispose();
        }

        public void Start()
        {
            _server.Start(new HostingApplication<TContext>(_contextFactory, _pipeline));
        }
    }
}
