using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ServerStack
{
    public class HostBuilder : IHostBuilder
    {
        public IHost Build()
        {
            return null;
        }

        public IHostBuilder Configure<TContext>(Action<IApplicationBuilder<TContext>> configureApplication)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            throw new NotImplementedException();
        }

        public string GetSetting(string key)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder UseServer<TServerFactory>()
        {
            throw new NotImplementedException();
        }

        public IHostBuilder UseServer(IServerFactory factory)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder UseSetting(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder UseStartup(Type startupType)
        {
            throw new NotImplementedException();
        }
    }

}
