using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Serialization
{
    public interface IFrameOutput
    {
        Task WriteAsync(Stream output, object value);
    }
}
