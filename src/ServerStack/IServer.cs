using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack
{
    public interface IServer : IDisposable
    {
        /// <summary>
        /// A collection of HTTP features of the server.
        /// </summary>
        IFeatureCollection Features { get; }

        /// <summary>
        /// Start the server with an HttpApplication.
        /// </summary>
        /// <param name="application">An instance of <see cref="IHttpApplication"/>.</param>
        void Start<TContext>(IApplication<TContext> application);
    }
}
