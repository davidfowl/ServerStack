using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace ServerStack
{
    public static class ServerHostingExtensions
    {
        /// <summary>
        /// Runs a web application and block the calling thread until host shutdown.
        /// </summary>
        /// <param name="host"></param>
        public static void Run<TContext>(this IServerHost<TContext> host)
        {
            using (var cts = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    cts.Cancel();

                    // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                host.Run(cts.Token, "Application started. Press Ctrl+C to shut down.");
            }
        }

        /// <summary>
        /// Runs a web application and block the calling thread until token is triggered or shutdown is triggered
        /// </summary>
        /// <param name="host"></param>
        /// <param name="token">The token to trigger shutdown</param>
        public static void Run<TContext>(this IServerHost<TContext> host, CancellationToken token)
        {
            host.Run(token, shutdownMessage: null);
        }

        private static void Run<TContext>(this IServerHost<TContext> host, CancellationToken token, string shutdownMessage)
        {
            using (host)
            {
                host.Start();

                var applicationLifetime = host.Services.GetService<IApplicationLifetime>();

                /*var serverAddresses = host.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
                if (serverAddresses != null)
                {
                    foreach (var address in serverAddresses)
                    {
                        Console.WriteLine($"Now listening on: {address}");
                    }
                }*/

                if (!string.IsNullOrEmpty(shutdownMessage))
                {
                    Console.WriteLine(shutdownMessage);
                }

                token.Register(state =>
                {
                    ((IApplicationLifetime)state).StopApplication();
                },
                applicationLifetime);

                applicationLifetime.ApplicationStopping.WaitHandle.WaitOne();
            }
        }
    }

    public interface IServerHost<TContext> : IDisposable
    {
        IFeatureCollection ServerFeatures { get; }

        IServiceProvider Services { get; }

        void Start();
    }
}