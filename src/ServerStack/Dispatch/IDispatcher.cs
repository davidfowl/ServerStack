using System;
using System.IO;
using System.Threading.Tasks;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public interface IDispatcher<T> : IObservable<Frame<T>>
    {
        Task Invoke(Stream stream);
    }
}
