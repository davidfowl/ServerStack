using System;

namespace ServerStack
{
    public static class HostingExtensions
    {
        public static void Run(this IHost host)
        {

        }
    }

    public interface IHost : IDisposable
    {
        IFeatureCollection ServerFeatures { get; }

        IServiceProvider Services { get; }

        void Start();
    }
}