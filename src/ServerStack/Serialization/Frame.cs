using System;
using System.IO;
using ServerStack.Infrastructure;

namespace ServerStack.Serialization
{
    public struct Frame<T>
    {
        public Frame(IOutputProducer output, T data)
        {
            Output = output;
            Data = data;
        }

        public IOutputProducer Output { get; }

        public T Data { get; }
    }
}
