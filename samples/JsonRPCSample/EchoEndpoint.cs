using System.Threading.Tasks;
using JsonRPC;

namespace JsonRPCSample
{
    public class EchoEndpoint : RpcEndpoint
    {
        public string Echo(string value)
        {
            return value;
        }
    }
}
