using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ServerStack
{
    public interface IServerFactory
    {
        /// <summary>
        /// Creates <see cref="IServer"/> based on the given configuration.
        /// </summary>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
        /// <returns>The created server.</returns>
        IServer CreateServer(IConfiguration configuration);
    }
}
