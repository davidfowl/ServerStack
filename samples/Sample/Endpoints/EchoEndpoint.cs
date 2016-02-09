using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRPC;

namespace Sample.Endpoints
{
    public class EchoEndpoint : RpcEndpoint
    {
        public string Echo(string value)
        {
            return value;
        }

        [JsonRPCMethod("ping")]
        public string Ping()
        {
            return "pong";
        }
    }
}
