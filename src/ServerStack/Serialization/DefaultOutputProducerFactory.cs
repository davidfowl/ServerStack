using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStack.Serialization
{

    public class DefaultOutputProducerFactory : IOutputProducerFactory
    {
        private readonly IFrameOutput _output;

        public DefaultOutputProducerFactory(IFrameOutput output)
        {
            _output = output;
        }


        public IOutputProducer Create(Stream stream)
        {
            return new DefaultOutputProducer(_output, stream);
        }
    }
}
