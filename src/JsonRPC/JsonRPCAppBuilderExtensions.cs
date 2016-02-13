using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ServerStack;
using ServerStack.Middleware;
using ServerStack.Protocols.Tcp;

namespace ServerStack
{
    public static class JsonRPCAppBuilderExtensions
    {
        public static IApplicationBuilder<TcpContext> UseJsonRpc(this IApplicationBuilder<TcpContext> app)
        {
            return app.UseDispatcher<JObject>();
        }
    }
}
