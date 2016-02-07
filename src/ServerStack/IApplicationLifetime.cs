using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerStack
{
    public interface IApplicationLifetime
    {
        /// <summary>
        /// Triggered when the application host has fully started and is about to wait
        /// for a graceful shutdown.
        /// </summary>
        CancellationToken ApplicationStarted { get; }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// Request may still be in flight. Shutdown will block until this event completes.
        /// </summary>
        /// <returns></returns>
        CancellationToken ApplicationStopping { get; }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// All requests should be complete at this point. Shutdown will block
        /// until this event completes.
        /// </summary>
        /// <returns></returns>
        CancellationToken ApplicationStopped { get; }

        /// <summary>
        /// Requests termination the current application.
        /// </summary>
        void StopApplication();
    }
}
