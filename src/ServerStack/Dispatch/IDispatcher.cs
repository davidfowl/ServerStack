using System;
using System.IO;
using System.Threading.Tasks;
using ServerStack.Serialization;

namespace ServerStack.Dispatch
{
    public interface IDispatcher<T>
    {
        event Action<IOutputProducer, T> Callback;

        event Action<Exception> OnError;

        event Action OnCompleted;

        Task Invoke(Stream stream);

    }
}
