using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Protocols.Tcp
{
    public interface ITcpApplicationBuilder : IApplicationBuilder<TcpContext>
    {
    }
}
