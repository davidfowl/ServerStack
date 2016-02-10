using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Serialization
{
    public interface IFrameHandler<TInput>
    {
        Task<object> OnFrame(TInput value);
    }
}
