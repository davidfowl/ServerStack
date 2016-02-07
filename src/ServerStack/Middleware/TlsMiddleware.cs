using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ServerStack.Features;
using ServerStack.Protocols.Tcp;

namespace ServerStack
{
    public class TlsMiddleware
    {
        private readonly X509Certificate2 _cert;
        private readonly Func<TcpContext, Task> _next;

        public TlsMiddleware(Func<TcpContext, Task> next, X509Certificate2 cert)
        {
            _next = next;
            _cert = cert;
        }

        public async Task Invoke(TcpContext context)
        {
            var sslStream = new SslStream(context.Body);

            await sslStream.AuthenticateAsServerAsync(_cert);

            context.Body = sslStream;

            await _next(context);
        }
    }

    public static class TlsMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseTls(this IApplicationBuilder<TcpContext> app, X509Certificate2 cert)
        {
            return app.Use(next => ctx => new TlsMiddleware(next, cert).Invoke(ctx));
        }
    }
}