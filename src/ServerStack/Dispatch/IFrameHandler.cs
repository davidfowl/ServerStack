using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Dispatch
{
    public interface IFrameHandler<TInput>
    {
        Task OnFrame(Stream output, TInput value);
    }
}
