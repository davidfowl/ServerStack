using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ServerStack.Features;

namespace ServerStack
{
    public class TlsMiddleware
    {
        private X509Certificate2 cert;
        private Func<IFeatureCollection, Task> next;

        public TlsMiddleware(Func<IFeatureCollection, Task> next, X509Certificate2 cert)
        {
            this.next = next;
            this.cert = cert;
        }

        public async Task Invoke(IFeatureCollection context)
        {
            var connection = context.Get<IConnectionFeature>();
            var sslStream = new SslStream(connection.Body);

            await sslStream.AuthenticateAsServerAsync(cert);

            connection.Body = sslStream;

            await next(context);
        }
    }

    public static class TlsMiddlewareExtensions
    {
        public static IApplicationBuilder<IFeatureCollection> UseTls(this IApplicationBuilder<IFeatureCollection> app, X509Certificate2 cert)
        {
            return app.Use(next => ctx => new TlsMiddleware(next, cert).Invoke(ctx));
        }
    }
}