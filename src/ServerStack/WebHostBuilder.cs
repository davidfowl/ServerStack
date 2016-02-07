using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerStack.Protocols.Http;

namespace ServerStack
{
    // ASP.NET esque API
    public class WebHostBuilder : ServerHostBuilder<HttpContext>
    {
    }
}
