using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Protocols.Http
{
    public interface IHttpApplicationBuilder : IApplicationBuilder<HttpContext>
    {
    }
}
